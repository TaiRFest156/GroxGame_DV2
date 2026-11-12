using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Manual de Grox: un libro con paginas que se pasan con animacion.
// Construye toda su interfaz por codigo (no hay que armar ningun Canvas a mano).
//
// USO RAPIDO
//  - Pon un objeto con este script en cada escena (menu Grox > Manual > ...).
//  - Niveles: introAutomatica = true  -> muestra "¡Aprendamos a jugar!" y abre el manual solo.
//  - Menu:    introAutomatica = false, crearBotonFlotante = false, y asigna tu boton en "botonManual".
//  - Desde otro script:  ManualSystem.AbrirManual();
[DisallowMultipleComponent]
public class ManualSystem : MonoBehaviour
{
    // ------------------------------------------------------------------ acceso global
    public static ManualSystem Instance { get; private set; }
    public static bool EstaAbierto { get; private set; }

    public static void AbrirManual()
    {
        if (Instance != null) Instance.Abrir();
    }

    // ------------------------------------------------------------------ configuracion
    [Header("Contenido")]
    public ManualData datos;

    [Header("Apertura")]
    [Tooltip("Pagina (empezando en 1) en la que se abre el manual. Ej: la pagina del nivel actual.")]
    public int paginaInicial = 1;
    [Tooltip("Para escenas de nivel: muestra el texto de bienvenida y abre el manual solo.")]
    public bool introAutomatica = true;
    public string textoIntro = "¡Aprendamos a jugar!";
    [Tooltip("Segundos de espera antes del texto (ej. para que termine un fundido de pantalla).")]
    public float esperaInicial = 1f;
    [Tooltip("Cuanto tiempo parpadea el texto antes de abrir el manual.")]
    public float duracionTextoIntro = 2.5f;
    [Tooltip("Si esta activo, al reintentar un nivel el manual NO se vuelve a abrir solo.")]
    public bool soloUnaVezPorSesion = true;

    [Header("Boton del manual")]
    [Tooltip("Opcional: un boton tuyo que ya este en la escena. Se conecta solo.")]
    public Button botonManual;
    [Tooltip("Si no asignas boton, crea uno flotante (solo en niveles).")]
    public bool crearBotonFlotante = true;
    [Tooltip("Opcional: tu propio icono (sprite) para el boton flotante.")]
    public Sprite iconoBoton;
    [Tooltip("Posicion del centro del boton medida desde la esquina superior derecha.")]
    public Vector2 posicionBoton = new Vector2(-230f, -80f);
    public float tamanoBoton = 110f;

    [Header("Aspecto")]
    public Color colorCubierta = new Color32(35, 27, 75, 255);
    public Color colorAcento = new Color32(46, 230, 166, 255);
    public Color colorPapel = new Color32(255, 244, 220, 255);
    public Color colorTexto = new Color32(43, 37, 64, 255);
    public Color colorAcentoPapel = new Color32(14, 159, 119, 255);
    [Tooltip("Opcional: tu fuente TextMeshPro. Si esta vacia usa la de por defecto.")]
    public TMP_FontAsset fuente;
    [Tooltip("Duracion total del giro de hoja (segundos).")]
    public float duracionVolteo = 0.45f;
    [Tooltip("Orden del Canvas del manual (mayor = se dibuja encima).")]
    public int ordenCanvas = 500;

    [Header("Sonidos (opcionales)")]
    public AudioClip sonidoAbrir;
    public AudioClip sonidoPagina;
    public AudioClip sonidoIntro;

    // ------------------------------------------------------------------ estado interno
    private class PageView
    {
        public RectTransform rect;
        public RectTransform contenido;
        public TextMeshProUGUI etiqueta;
        public TextMeshProUGUI titulo;
        public TextMeshProUGUI cuerpo;
        public TextMeshProUGUI numero;
        public Image imagen;
        public Image sombra;
    }

    private RectTransform canvasRT;
    private GameObject root;
    private CanvasGroup rootGroup;
    private RectTransform book;
    private PageView izq, der, hojaIzq, hojaDer;
    private Button btnAnterior, btnSiguiente;
    private Toggle toggleAuto;

    private GameObject introRoot;
    private CanvasGroup introGroup;
    private RectTransform introBanner;

    private RectTransform botonRT;
    private Vector3 botonEscalaOriginal = Vector3.one;
    private Image halo;

    private AudioSource audioSrc;
    private Coroutine flipCo, animCo, parpadeoCo;

    private int spreadIndex;
    private int spreadCount = 1;
    private bool animando;
    private bool cerrando;

    private bool pausadoPorManual;
    private float escalaGuardada = 1f;

    // ================================================================== ciclo de vida
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            enabled = false;
            Destroy(gameObject);
            return;
        }

        Instance = this;
        EstaAbierto = false;

        audioSrc = gameObject.AddComponent<AudioSource>();
        audioSrc.playOnAwake = false;
        audioSrc.spatialBlend = 0f;

        AsegurarEventSystem();
        ConstruirUI();
    }

    private void Start()
    {
        if (!introAutomatica) return;
        if (datos == null || datos.paginas.Count == 0) return;
        if (!ManualSettings.AbrirAutomatico) return;

        string escena = SceneManager.GetActiveScene().name;
        if (soloUnaVezPorSesion && ManualSettings.IntroYaMostrada(escena)) return;

        StartCoroutine(SecuenciaInicio(escena));
    }

    private void Update()
    {
        if (EstaAbierto && TeclaAtras()) Cerrar();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            EstaAbierto = false;
        }

        if (pausadoPorManual)
        {
            Time.timeScale = escalaGuardada;
            pausadoPorManual = false;
        }

        if (botonManual != null) botonManual.onClick.RemoveListener(Abrir);
    }

    private static bool TeclaAtras()
    {
#if ENABLE_INPUT_SYSTEM
        var kb = UnityEngine.InputSystem.Keyboard.current;
        return kb != null && kb.escapeKey.wasPressedThisFrame;
#elif ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(KeyCode.Escape);
#else
        return false;
#endif
    }

    // ================================================================== API publica
    public void Abrir()
    {
        AbrirEnPagina(paginaInicial);
    }

    // pagina empieza en 1
    public void AbrirEnPagina(int pagina)
    {
        if (EstaAbierto || cerrando) return;
        if (datos == null || datos.paginas.Count == 0)
        {
            Debug.LogWarning("ManualSystem: no hay datos o paginas en el manual.");
            return;
        }

        DetenerParpadeo();
        Pausar();
        EstaAbierto = true;

        if (flipCo != null) { StopCoroutine(flipCo); flipCo = null; }
        animando = false;

        spreadCount = Mathf.Max(1, (datos.paginas.Count + 1) / 2);
        spreadIndex = Mathf.Clamp((pagina - 1) / 2, 0, spreadCount - 1);

        Rellenar(izq, spreadIndex * 2);
        Rellenar(der, spreadIndex * 2 + 1);
        hojaIzq.rect.gameObject.SetActive(false);
        hojaDer.rect.gameObject.SetActive(false);
        ActualizarNavegacion();

        toggleAuto.SetIsOnWithoutNotify(ManualSettings.AbrirAutomatico);

        root.SetActive(true);
        Sonido(sonidoAbrir);

        if (animCo != null) StopCoroutine(animCo);
        animCo = StartCoroutine(Entrada());
    }

    public void Cerrar()
    {
        if (!EstaAbierto || cerrando) return;
        StartCoroutine(CerrarRutina());
    }

    public void Siguiente()
    {
        if (!EstaAbierto || animando || spreadIndex >= spreadCount - 1) return;
        flipCo = StartCoroutine(Voltear(spreadIndex + 1));
    }

    public void Anterior()
    {
        if (!EstaAbierto || animando || spreadIndex <= 0) return;
        flipCo = StartCoroutine(Voltear(spreadIndex - 1));
    }

    // ================================================================== pausa
    private void Pausar()
    {
        if (pausadoPorManual) return;
        escalaGuardada = Time.timeScale;
        Time.timeScale = 0f;
        pausadoPorManual = true;
    }

    private void Reanudar()
    {
        if (!pausadoPorManual) return;
        Time.timeScale = escalaGuardada;
        pausadoPorManual = false;
    }

    // ================================================================== secuencia de inicio de nivel
    private IEnumerator SecuenciaInicio(string escena)
    {
        // Esperar un frame: asi todos los Start() de la escena ya corrieron
        // (algunos, como el del Nivel 2, ponen Time.timeScale = 1).
        yield return null;

        ManualSettings.MarcarIntroMostrada(escena);
        Pausar();   // el juego queda congelado mientras se lee, asi no se pierde tiempo del nivel

        yield return new WaitForSecondsRealtime(Mathf.Max(0f, esperaInicial));

        IniciarParpadeo();
        Sonido(sonidoIntro);
        yield return AnimarIntro();

        Abrir();
    }

    private IEnumerator AnimarIntro()
    {
        introRoot.SetActive(true);
        introGroup.alpha = 0f;

        // Entrada con rebote
        float t = 0f;
        const float entrada = 0.4f;
        while (t < entrada)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / entrada);
            introGroup.alpha = k;
            introBanner.localScale = Vector3.one * Mathf.LerpUnclamped(0.5f, 1f, EaseOutBack(k));
            yield return null;
        }

        // Parpadeo
        t = 0f;
        while (t < duracionTextoIntro)
        {
            t += Time.unscaledDeltaTime;
            float onda = 0.5f + 0.5f * Mathf.Sin(t * 9f);
            introGroup.alpha = Mathf.Lerp(0.45f, 1f, onda);
            introBanner.localScale = Vector3.one * (1f + 0.03f * onda);
            yield return null;
        }

        // Salida
        t = 0f;
        const float salida = 0.25f;
        while (t < salida)
        {
            t += Time.unscaledDeltaTime;
            introGroup.alpha = 1f - Mathf.Clamp01(t / salida);
            yield return null;
        }

        introRoot.SetActive(false);
    }

    // ================================================================== parpadeo del icono
    private void IniciarParpadeo()
    {
        if (botonRT == null) return;
        DetenerParpadeo();

        if (halo == null)
        {
            var go = new GameObject("HaloManual", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            go.GetComponent<LayoutElement>().ignoreLayout = true;

            var hrt = (RectTransform)go.transform;
            hrt.SetParent(botonRT.parent, false);
            hrt.anchorMin = botonRT.anchorMin;
            hrt.anchorMax = botonRT.anchorMax;
            hrt.pivot = botonRT.pivot;
            hrt.anchoredPosition = botonRT.anchoredPosition;
            hrt.sizeDelta = botonRT.sizeDelta;
            hrt.SetSiblingIndex(botonRT.GetSiblingIndex());   // justo detras del boton

            halo = go.GetComponent<Image>();
            halo.sprite = ManualSpriteFactory.Circulo();
            halo.raycastTarget = false;
        }

        halo.gameObject.SetActive(true);
        parpadeoCo = StartCoroutine(RutinaParpadeo());
    }

    private void DetenerParpadeo()
    {
        if (parpadeoCo != null) { StopCoroutine(parpadeoCo); parpadeoCo = null; }
        if (botonRT != null) botonRT.localScale = botonEscalaOriginal;
        if (halo != null) halo.gameObject.SetActive(false);
    }

    private IEnumerator RutinaParpadeo()
    {
        float t = 0f;
        while (true)
        {
            t += Time.unscaledDeltaTime;

            botonRT.localScale = botonEscalaOriginal * (1f + 0.10f * Mathf.Abs(Mathf.Sin(t * 5f)));

            float ciclo = t % 1f;
            halo.rectTransform.localScale = Vector3.one * (1f + ciclo * 0.9f);
            halo.color = new Color(colorAcento.r, colorAcento.g, colorAcento.b, (1f - ciclo) * 0.7f);

            yield return null;
        }
    }

    // ================================================================== animaciones del libro
    private IEnumerator Entrada()
    {
        const float dur = 0.35f;
        float t = 0f;
        rootGroup.alpha = 0f;
        book.localScale = Vector3.one * 0.85f;

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / dur);
            rootGroup.alpha = k;
            book.localScale = Vector3.one * Mathf.LerpUnclamped(0.85f, 1f, EaseOutBack(k));
            yield return null;
        }

        rootGroup.alpha = 1f;
        book.localScale = Vector3.one;
        animCo = null;
    }

    private IEnumerator CerrarRutina()
    {
        cerrando = true;

        if (flipCo != null) { StopCoroutine(flipCo); flipCo = null; }
        if (animCo != null) { StopCoroutine(animCo); animCo = null; }
        animando = false;

        const float dur = 0.2f;
        float t = 0f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / dur);
            rootGroup.alpha = 1f - k;
            book.localScale = Vector3.one * Mathf.Lerp(1f, 0.92f, k);
            yield return null;
        }

        root.SetActive(false);
        EstaAbierto = false;
        cerrando = false;
        Reanudar();
    }

    // Giro de hoja: la hoja se pliega hacia el lomo y se despliega del otro lado.
    private IEnumerator Voltear(int destino)
    {
        animando = true;
        bool adelante = destino > spreadIndex;

        int viejaIzq = spreadIndex * 2;
        int viejaDer = viejaIzq + 1;
        int nuevaIzq = destino * 2;
        int nuevaDer = nuevaIzq + 1;
        float mitad = Mathf.Max(0.05f, duracionVolteo * 0.5f);

        Sonido(sonidoPagina);

        if (adelante)
        {
            Rellenar(der, nuevaDer);                 // debajo ya esta la pagina nueva
            Rellenar(hojaDer, viejaDer);
            AplicarEscala(hojaDer, 1f);
            hojaDer.rect.gameObject.SetActive(true);
            yield return Escalar(hojaDer, 1f, 0f, mitad, false);
            hojaDer.rect.gameObject.SetActive(false);

            Rellenar(hojaIzq, nuevaIzq);
            AplicarEscala(hojaIzq, 0f);
            hojaIzq.rect.gameObject.SetActive(true);
            yield return Escalar(hojaIzq, 0f, 1f, mitad, true);
            Rellenar(izq, nuevaIzq);
            hojaIzq.rect.gameObject.SetActive(false);
        }
        else
        {
            Rellenar(izq, nuevaIzq);
            Rellenar(hojaIzq, viejaIzq);
            AplicarEscala(hojaIzq, 1f);
            hojaIzq.rect.gameObject.SetActive(true);
            yield return Escalar(hojaIzq, 1f, 0f, mitad, false);
            hojaIzq.rect.gameObject.SetActive(false);

            Rellenar(hojaDer, nuevaDer);
            AplicarEscala(hojaDer, 0f);
            hojaDer.rect.gameObject.SetActive(true);
            yield return Escalar(hojaDer, 0f, 1f, mitad, true);
            Rellenar(der, nuevaDer);
            hojaDer.rect.gameObject.SetActive(false);
        }

        spreadIndex = destino;
        ActualizarNavegacion();
        animando = false;
        flipCo = null;
    }

    private IEnumerator Escalar(PageView hoja, float desde, float hasta, float dur, bool desacelerar)
    {
        float t = 0f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / dur);
            float e = desacelerar ? 1f - (1f - k) * (1f - k) : k * k;
            AplicarEscala(hoja, Mathf.Lerp(desde, hasta, e));
            yield return null;
        }
        AplicarEscala(hoja, hasta);
    }

    private void AplicarEscala(PageView h, float s)
    {
        s = Mathf.Max(0.001f, s);
        h.rect.localScale = new Vector3(s, 1f + 0.03f * (1f - s), 1f);
        h.sombra.color = new Color(0f, 0f, 0f, (1f - s) * 0.5f);   // se oscurece al ponerse de canto
    }

    private static float EaseOutBack(float x)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }

    // ================================================================== contenido
    private void Rellenar(PageView v, int idx)
    {
        bool hay = datos != null && idx >= 0 && idx < datos.paginas.Count;
        v.contenido.gameObject.SetActive(hay);
        if (!hay) return;   // hoja en blanco (cuando el total de paginas es impar)

        ManualPage p = datos.paginas[idx];
        bool conImagen = p.imagen != null;

        v.etiqueta.text = string.IsNullOrEmpty(p.etiqueta) ? "" : p.etiqueta.ToUpperInvariant();
        v.titulo.text = p.titulo;
        v.cuerpo.text = p.texto;
        v.numero.text = (idx + 1).ToString();

        v.imagen.gameObject.SetActive(conImagen);
        if (conImagen) v.imagen.sprite = p.imagen;

        // Si hay imagen, el texto baja para dejarle lugar
        SetAnchors(v.cuerpo.rectTransform,
                   new Vector2(0.09f, 0.09f),
                   new Vector2(0.91f, conImagen ? 0.43f : 0.77f));
    }

    private void ActualizarNavegacion()
    {
        btnAnterior.gameObject.SetActive(spreadIndex > 0);
        btnSiguiente.gameObject.SetActive(spreadIndex < spreadCount - 1);
    }

    private void Sonido(AudioClip clip)
    {
        if (clip != null && audioSrc != null) audioSrc.PlayOneShot(clip);
    }

    // ================================================================== construccion de la UI
    private void AsegurarEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null) return;

        var go = new GameObject("EventSystem", typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM
        var modulo = go.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        modulo.AssignDefaultActions();
#else
        go.AddComponent<StandaloneInputModule>();
#endif
    }

    private void ConstruirUI()
    {
        // ---- Canvas propio
        var cgo = new GameObject("ManualCanvas", typeof(RectTransform), typeof(Canvas),
                                 typeof(CanvasScaler), typeof(GraphicRaycaster));
        cgo.transform.SetParent(transform, false);

        var canvas = cgo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = ordenCanvas;

        var scaler = cgo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        canvasRT = (RectTransform)cgo.transform;

        // ---- Boton del manual (primero, para que el fondo oscuro lo tape al abrir)
        if (botonManual != null)
        {
            botonRT = botonManual.GetComponent<RectTransform>();
            botonManual.onClick.AddListener(Abrir);
        }
        else if (crearBotonFlotante)
        {
            CrearBotonFlotante();
        }
        if (botonRT != null) botonEscalaOriginal = botonRT.localScale;

        ConstruirIntro();
        ConstruirLibro();
    }

    private void CrearBotonFlotante()
    {
        RectTransform interior;
        Button b = NuevoBotonRedondo("BotonManual", canvasRT, new Vector2(1f, 1f), posicionBoton,
                                     tamanoBoton, colorAcento, colorCubierta, out interior);

        if (iconoBoton != null)
        {
            Image ic = NuevaImagen("Icono", interior, new Vector2(0.18f, 0.18f), new Vector2(0.82f, 0.82f),
                                   Color.white, iconoBoton, false);
            ic.preserveAspect = true;
        }
        else
        {
            TextMeshProUGUI signo = NuevoTexto("Signo", interior, Vector2.zero, Vector2.one, 20f, 90f,
                                               FontStyles.Bold, TextAlignmentOptions.Center, colorCubierta);
            signo.text = "?";
        }

        botonManual = b;
        botonRT = (RectTransform)b.transform;
        b.onClick.AddListener(Abrir);
    }

    private void ConstruirIntro()
    {
        RectTransform introRT = NuevoRect("Intro", canvasRT, Vector2.zero, Vector2.one);
        Image bloqueo = introRT.gameObject.AddComponent<Image>();
        bloqueo.color = new Color(0f, 0f, 0f, 0f);
        bloqueo.raycastTarget = true;   // evita toques accidentales mientras aparece el texto
        introGroup = introRT.gameObject.AddComponent<CanvasGroup>();
        introRoot = introRT.gameObject;

        RectTransform fondo = NuevoPanelConBorde("Banner", introRT, new Vector2(0.20f, 0.64f),
                                                 new Vector2(0.80f, 0.84f), colorCubierta, colorAcento, 8f);
        introBanner = (RectTransform)fondo.parent;

        TextMeshProUGUI txt = NuevoTexto("Texto", fondo, Vector2.zero, Vector2.one, 30f, 110f,
                                         FontStyles.Bold, TextAlignmentOptions.Center, Color.white);
        txt.rectTransform.offsetMin = new Vector2(24f, 12f);
        txt.rectTransform.offsetMax = new Vector2(-24f, -12f);
        txt.text = textoIntro;

        introRoot.SetActive(false);
    }

    private void ConstruirLibro()
    {
        // ---- Fondo oscuro
        RectTransform rootRT = NuevoRect("Manual", canvasRT, Vector2.zero, Vector2.one);
        Image dim = rootRT.gameObject.AddComponent<Image>();
        dim.color = new Color(0f, 0f, 0f, 0.78f);
        dim.raycastTarget = true;   // bloquea el juego de atras
        rootGroup = rootRT.gameObject.AddComponent<CanvasGroup>();
        root = rootRT.gameObject;

        // ---- Libro (mantiene proporcion en cualquier pantalla)
        RectTransform area = NuevoRect("AreaLibro", rootRT, new Vector2(0.03f, 0.115f), new Vector2(0.97f, 0.965f));
        book = NuevoRect("Libro", area, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        var arf = book.gameObject.AddComponent<AspectRatioFitter>();
        arf.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        arf.aspectRatio = 1.9f;

        // Cubierta con borde de color; ademas recibe el deslizamiento del dedo
        NuevoPanelConBorde("Cubierta", book, Vector2.zero, Vector2.one, colorCubierta, colorAcento, 8f);
        Image marco = book.GetChild(0).GetComponent<Image>();
        marco.raycastTarget = true;
        var swipe = marco.gameObject.AddComponent<ManualSwipe>();
        swipe.alDeslizarIzquierda = Siguiente;
        swipe.alDeslizarDerecha = Anterior;

        // ---- Paginas y hojas que giran
        Vector2 izqMin = new Vector2(0.04f, 0.06f), izqMax = new Vector2(0.4975f, 0.94f);
        Vector2 derMin = new Vector2(0.5025f, 0.06f), derMax = new Vector2(0.96f, 0.94f);

        izq = CrearPagina("PaginaIzq", book, izqMin, izqMax, new Vector2(0.5f, 0.5f));
        der = CrearPagina("PaginaDer", book, derMin, derMax, new Vector2(0.5f, 0.5f));
        hojaIzq = CrearPagina("HojaIzq", book, izqMin, izqMax, new Vector2(1f, 0.5f));   // gira sobre el lomo
        hojaDer = CrearPagina("HojaDer", book, derMin, derMax, new Vector2(0f, 0.5f));
        hojaIzq.rect.gameObject.SetActive(false);
        hojaDer.rect.gameObject.SetActive(false);

        // ---- Lomo (sombra central)
        NuevaImagen("LomoSombra", book, new Vector2(0.4925f, 0.06f), new Vector2(0.5075f, 0.94f),
                    new Color(0f, 0f, 0f, 0.22f), null, false);
        NuevaImagen("LomoLinea", book, new Vector2(0.4992f, 0.06f), new Vector2(0.5008f, 0.94f),
                    new Color(0f, 0f, 0f, 0.5f), null, false);

        // ---- Flechas y cerrar
        RectTransform interior;

        btnAnterior = NuevoBotonRedondo("Anterior", book, new Vector2(0f, 0.5f), new Vector2(-6f, 0f),
                                        96f, colorAcento, colorCubierta, out interior);
        Image fl1 = NuevaImagen("Flecha", interior, new Vector2(0.2f, 0.2f), new Vector2(0.8f, 0.8f),
                                colorCubierta, ManualSpriteFactory.Triangulo(), false);
        fl1.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
        btnAnterior.onClick.AddListener(Anterior);

        btnSiguiente = NuevoBotonRedondo("Siguiente", book, new Vector2(1f, 0.5f), new Vector2(6f, 0f),
                                         96f, colorAcento, colorCubierta, out interior);
        NuevaImagen("Flecha", interior, new Vector2(0.2f, 0.2f), new Vector2(0.8f, 0.8f),
                    colorCubierta, ManualSpriteFactory.Triangulo(), false);
        btnSiguiente.onClick.AddListener(Siguiente);

        Button cerrar = NuevoBotonRedondo("Cerrar", book, new Vector2(1f, 1f), new Vector2(-8f, -8f),
                                          84f, new Color32(255, 94, 98, 255), colorCubierta, out interior);
        TextMeshProUGUI x = NuevoTexto("X", interior, Vector2.zero, Vector2.one, 20f, 70f,
                                       FontStyles.Bold, TextAlignmentOptions.Center, Color.white);
        x.text = "X";
        cerrar.onClick.AddListener(Cerrar);

        // ---- Opcion: abrir automaticamente (se guarda en el dispositivo)
        ConstruirOpcionAutomatica(rootRT);

        root.SetActive(false);
    }

    private void ConstruirOpcionAutomatica(RectTransform rootRT)
    {
        RectTransform fila = NuevoRect("OpcionAutomatica", rootRT, new Vector2(0.26f, 0.012f), new Vector2(0.74f, 0.095f));
        Image area = fila.gameObject.AddComponent<Image>();
        area.color = new Color(0f, 0f, 0f, 0f);
        area.raycastTarget = true;   // toda la fila es clickeable

        RectTransform caja = NuevoRect("Caja", fila, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f));
        caja.pivot = new Vector2(0f, 0.5f);
        caja.sizeDelta = new Vector2(54f, 54f);
        caja.anchoredPosition = Vector2.zero;
        Image cajaImg = caja.gameObject.AddComponent<Image>();
        cajaImg.sprite = ManualSpriteFactory.RectRedondeado();
        cajaImg.type = Image.Type.Sliced;
        cajaImg.color = Color.white;
        cajaImg.raycastTarget = false;

        Image check = NuevaImagen("Check", caja, Vector2.zero, Vector2.one, colorAcentoPapel,
                                  ManualSpriteFactory.RectRedondeado(), true);
        check.rectTransform.offsetMin = new Vector2(9f, 9f);
        check.rectTransform.offsetMax = new Vector2(-9f, -9f);

        TextMeshProUGUI etiqueta = NuevoTexto("Etiqueta", fila, Vector2.zero, Vector2.one, 18f, 40f,
                                              FontStyles.Normal, TextAlignmentOptions.MidlineLeft, Color.white);
        etiqueta.rectTransform.offsetMin = new Vector2(72f, 0f);
        etiqueta.text = "Abrir el manual automáticamente al empezar un nivel";

        toggleAuto = fila.gameObject.AddComponent<Toggle>();
        toggleAuto.targetGraphic = cajaImg;
        toggleAuto.graphic = check;
        toggleAuto.isOn = ManualSettings.AbrirAutomatico;
        toggleAuto.onValueChanged.AddListener(v => ManualSettings.AbrirAutomatico = v);
    }

    private PageView CrearPagina(string nombre, Transform padre, Vector2 min, Vector2 max, Vector2 pivote)
    {
        var go = new GameObject(nombre, typeof(RectTransform));
        var rt = (RectTransform)go.transform;
        rt.SetParent(padre, false);
        rt.pivot = pivote;               // primero el pivote, despues las anclas
        SetAnchors(rt, min, max);

        var v = new PageView { rect = rt };

        NuevaImagen("Papel", rt, Vector2.zero, Vector2.one, colorPapel, null, false);

        RectTransform cont = NuevoRect("Contenido", rt, Vector2.zero, Vector2.one);
        v.contenido = cont;

        v.etiqueta = NuevoTexto("Etiqueta", cont, new Vector2(0.09f, 0.915f), new Vector2(0.91f, 0.975f),
                                14f, 26f, FontStyles.Bold, TextAlignmentOptions.MidlineLeft, colorAcentoPapel);
        v.titulo = NuevoTexto("Titulo", cont, new Vector2(0.09f, 0.805f), new Vector2(0.91f, 0.915f),
                              24f, 54f, FontStyles.Bold, TextAlignmentOptions.MidlineLeft, colorTexto);
        NuevaImagen("Linea", cont, new Vector2(0.09f, 0.795f), new Vector2(0.91f, 0.803f),
                    colorAcentoPapel, null, false);

        v.imagen = NuevaImagen("Imagen", cont, new Vector2(0.2f, 0.45f), new Vector2(0.8f, 0.78f),
                               Color.white, null, false);
        v.imagen.preserveAspect = true;

        v.cuerpo = NuevoTexto("Cuerpo", cont, new Vector2(0.09f, 0.09f), new Vector2(0.91f, 0.77f),
                              16f, 36f, FontStyles.Normal, TextAlignmentOptions.TopLeft, colorTexto);
        v.cuerpo.paragraphSpacing = 10f;

        v.numero = NuevoTexto("Numero", cont, new Vector2(0.35f, 0.015f), new Vector2(0.65f, 0.07f),
                              12f, 24f, FontStyles.Normal, TextAlignmentOptions.Center,
                              new Color(colorTexto.r, colorTexto.g, colorTexto.b, 0.6f));

        v.sombra = NuevaImagen("Sombra", rt, Vector2.zero, Vector2.one, new Color(0f, 0f, 0f, 0f), null, false);
        return v;
    }

    // ------------------------------------------------------------------ ayudas de UI
    private static void SetAnchors(RectTransform rt, Vector2 min, Vector2 max)
    {
        rt.anchorMin = min;
        rt.anchorMax = max;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static RectTransform NuevoRect(string nombre, Transform padre, Vector2 min, Vector2 max)
    {
        var go = new GameObject(nombre, typeof(RectTransform));
        var rt = (RectTransform)go.transform;
        rt.SetParent(padre, false);
        SetAnchors(rt, min, max);
        return rt;
    }

    private static Image NuevaImagen(string nombre, Transform padre, Vector2 min, Vector2 max,
                                     Color color, Sprite sprite, bool sliced)
    {
        RectTransform rt = NuevoRect(nombre, padre, min, max);
        Image img = rt.gameObject.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = false;
        if (sprite != null)
        {
            img.sprite = sprite;
            img.type = sliced ? Image.Type.Sliced : Image.Type.Simple;
        }
        return img;
    }

    private TextMeshProUGUI NuevoTexto(string nombre, Transform padre, Vector2 min, Vector2 max,
                                       float tamMin, float tamMax, FontStyles estilo,
                                       TextAlignmentOptions alineacion, Color color)
    {
        RectTransform rt = NuevoRect(nombre, padre, min, max);
        TextMeshProUGUI t = rt.gameObject.AddComponent<TextMeshProUGUI>();
        if (fuente != null) t.font = fuente;
        t.enableAutoSizing = true;
        t.fontSizeMin = tamMin;
        t.fontSizeMax = tamMax;
        t.fontStyle = estilo;
        t.alignment = alineacion;
        t.color = color;
        t.richText = true;
        t.raycastTarget = false;
        return t;
    }

    // Devuelve el panel interior; el marco (borde) es su padre
    private RectTransform NuevoPanelConBorde(string nombre, Transform padre, Vector2 min, Vector2 max,
                                             Color fondo, Color borde, float grosor)
    {
        Image marco = NuevaImagen(nombre + "_Marco", padre, min, max, borde,
                                  ManualSpriteFactory.RectRedondeado(), true);
        Image interior = NuevaImagen(nombre, marco.transform, Vector2.zero, Vector2.one, fondo,
                                     ManualSpriteFactory.RectRedondeado(), true);
        interior.rectTransform.offsetMin = new Vector2(grosor, grosor);
        interior.rectTransform.offsetMax = new Vector2(-grosor, -grosor);
        return interior.rectTransform;
    }

    private Button NuevoBotonRedondo(string nombre, Transform padre, Vector2 ancla, Vector2 pos, float tam,
                                     Color relleno, Color borde, out RectTransform interior)
    {
        RectTransform rt = NuevoRect(nombre, padre, ancla, ancla);
        rt.sizeDelta = new Vector2(tam, tam);
        rt.anchoredPosition = pos;

        Image anillo = rt.gameObject.AddComponent<Image>();
        anillo.sprite = ManualSpriteFactory.Circulo();
        anillo.color = borde;
        anillo.raycastTarget = true;

        Image centro = NuevaImagen("Centro", rt, Vector2.zero, Vector2.one, relleno,
                                   ManualSpriteFactory.Circulo(), false);
        centro.rectTransform.offsetMin = new Vector2(7f, 7f);
        centro.rectTransform.offsetMax = new Vector2(-7f, -7f);
        interior = centro.rectTransform;

        Button boton = rt.gameObject.AddComponent<Button>();
        boton.targetGraphic = centro;   // el color de "presionado" se aplica al centro
        ColorBlock cb = boton.colors;
        cb.normalColor = Color.white;
        cb.highlightedColor = new Color(0.92f, 0.92f, 0.92f, 1f);
        cb.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        cb.selectedColor = Color.white;
        cb.fadeDuration = 0.08f;
        boton.colors = cb;
        return boton;
    }
}

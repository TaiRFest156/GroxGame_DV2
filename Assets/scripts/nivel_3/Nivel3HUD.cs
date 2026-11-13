using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Interfaz del Nivel 3: panel tipo "tableta" con el pedido, instrucciones, fusibles,
// barra de precision y progreso del Simon. Se construye entera por codigo.
// Depende de ManualSpriteFactory (del sistema de manual).
public class Nivel3HUD : MonoBehaviour
{
    [Header("Aspecto")]
    public Color colorPanel = new Color32(24, 28, 52, 240);
    public Color colorBorde = new Color32(46, 230, 166, 255);
    public Color colorOro = new Color32(255, 214, 10, 255);
    public Color colorFusibleApagado = new Color32(80, 48, 60, 255);
    public TMP_FontAsset fuente;
    public int ordenCanvas = 100;

    public event Action AlPedirRepetir;

    public float DistanciaAlCentro { get { return Mathf.Abs(marcadorPos - zonaCentro); } }

    // ---- elementos
    private RectTransform cRT;
    private TextMeshProUGUI txtPedido, txtInstr, toastTxt, datoTitulo, datoTexto, resTitulo, resDetalle;
    private RectTransform zonaBarra, zonaVerde, marcadorRT, zonaSimon, timerFill, datoFill;
    private readonly Image[] puntos = new Image[8];
    private readonly Image[] fusiblesImg = new Image[5];
    private readonly Image[] estrellasImg = new Image[3];
    private Button btnRepetir, btnContinuar;
    private Image flashImg;
    private CanvasGroup toastGroup, datoGroup;
    private GameObject datoRoot, resRoot;
    private Coroutine toastCo;

    // ---- estado de la barra
    private bool barraActiva;
    private float marcadorPos, dirMarcador = 1f, velBarra = 1f, zonaCentro = 0.5f;
    private bool cerrarDato;
    private static Sprite estrellaSprite;

    private void Awake()
    {
        Construir();
    }

    private void Update()
    {
        if (!barraActiva || Time.timeScale == 0f) return;

        marcadorPos += dirMarcador * velBarra * Time.deltaTime;
        if (marcadorPos >= 1f) { marcadorPos = 1f; dirMarcador = -1f; }
        else if (marcadorPos <= 0f) { marcadorPos = 0f; dirMarcador = 1f; }

        marcadorRT.anchorMin = new Vector2(marcadorPos, 0f);
        marcadorRT.anchorMax = new Vector2(marcadorPos, 1f);
    }

    // ================================================================== API
    public void SetPedido(int numero, int total, string producto)
    {
        txtPedido.text = "PEDIDO " + numero + "/" + total + "  -  " + producto.ToUpperInvariant();
    }

    public void SetInstruccion(string texto)
    {
        txtInstr.text = texto;
    }

    public void SetFusibles(int restantes, int maximo)
    {
        for (int i = 0; i < fusiblesImg.Length; i++)
        {
            bool existe = i < maximo;
            fusiblesImg[i].gameObject.SetActive(existe);
            if (!existe) continue;

            bool encendido = i < restantes;
            Color nuevo = encendido ? colorOro : colorFusibleApagado;
            if (fusiblesImg[i].color != nuevo && !encendido) StartCoroutine(PopFusible(fusiblesImg[i].rectTransform));
            fusiblesImg[i].color = nuevo;
        }
    }

    private IEnumerator PopFusible(RectTransform rt)
    {
        float t = 0f;
        while (t < 0.35f)
        {
            t += Time.unscaledDeltaTime;
            rt.localScale = Vector3.one * (1f + 0.5f * Mathf.Sin(Mathf.Clamp01(t / 0.35f) * Mathf.PI));
            yield return null;
        }
        rt.localScale = Vector3.one;
    }

    // ---- barra de precision de la palanca
    public void MostrarBarra(float centro, float ancho, float velocidad)
    {
        zonaSimon.gameObject.SetActive(false);
        zonaBarra.gameObject.SetActive(true);

        zonaCentro = Mathf.Clamp(centro, ancho * 0.5f, 1f - ancho * 0.5f);
        zonaVerde.anchorMin = new Vector2(zonaCentro - ancho * 0.5f, 0f);
        zonaVerde.anchorMax = new Vector2(zonaCentro + ancho * 0.5f, 1f);

        velBarra = velocidad;
        marcadorPos = 0f;
        dirMarcador = 1f;
        barraActiva = true;
    }

    public void OcultarBarra()
    {
        barraActiva = false;
        zonaBarra.gameObject.SetActive(false);
    }

    // ---- progreso del Simon
    public void MostrarSimon(int pasos)
    {
        OcultarBarra();
        zonaSimon.gameObject.SetActive(true);

        for (int i = 0; i < puntos.Length; i++)
        {
            bool usa = i < pasos;
            puntos[i].gameObject.SetActive(usa);
            if (!usa) continue;
            float x = (i + 0.5f) / pasos;
            RectTransform rt = puntos[i].rectTransform;
            rt.anchorMin = new Vector2(x, 0.5f);
            rt.anchorMax = new Vector2(x, 0.5f);
            rt.anchoredPosition = Vector2.zero;
        }
        ResetPasos();
    }

    public void OcultarSimon()
    {
        zonaSimon.gameObject.SetActive(false);
        MostrarBotonRepetir(false);
    }

    public void ResetPasos()
    {
        for (int i = 0; i < puntos.Length; i++) puntos[i].color = new Color(1f, 1f, 1f, 0.25f);
        SetTiempo(1f);
    }

    public void MarcarPaso(int indice, Color color)
    {
        if (indice >= 0 && indice < puntos.Length) puntos[indice].color = color;
    }

    public void SetTiempo(float fraccion)
    {
        timerFill.anchorMax = new Vector2(Mathf.Clamp01(fraccion), 1f);
    }

    public void MostrarBotonRepetir(bool visible)
    {
        btnRepetir.gameObject.SetActive(visible);
    }

    // ---- mensajes grandes
    public void Toast(string texto, Color color)
    {
        if (toastCo != null) StopCoroutine(toastCo);
        toastCo = StartCoroutine(ToastRutina(texto, color));
    }

    private IEnumerator ToastRutina(string texto, Color color)
    {
        toastTxt.text = texto;
        toastTxt.color = color;
        RectTransform rt = toastTxt.rectTransform;
        toastGroup.alpha = 1f;

        float t = 0f;
        while (t < 0.25f)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / 0.25f);
            rt.localScale = Vector3.one * Mathf.Lerp(0.5f, 1.1f, k);
            yield return null;
        }
        rt.localScale = Vector3.one;

        yield return new WaitForSecondsRealtime(0.9f);

        t = 0f;
        while (t < 0.3f)
        {
            t += Time.unscaledDeltaTime;
            toastGroup.alpha = 1f - Mathf.Clamp01(t / 0.3f);
            yield return null;
        }
        toastGroup.alpha = 0f;
        toastCo = null;
    }

    // Destello de pantalla; 'alPico' se ejecuta cuando la pantalla esta cubierta (ahi se hace el teletransporte)
    public IEnumerator Pantallazo(Color color, float subir, float bajar, Action alPico)
    {
        float t = 0f;
        while (t < subir)
        {
            t += Time.unscaledDeltaTime;
            flashImg.color = new Color(color.r, color.g, color.b, Mathf.Clamp01(t / subir));
            yield return null;
        }
        flashImg.color = new Color(color.r, color.g, color.b, 1f);

        if (alPico != null) alPico();
        yield return new WaitForSecondsRealtime(0.08f);

        t = 0f;
        while (t < bajar)
        {
            t += Time.unscaledDeltaTime;
            flashImg.color = new Color(color.r, color.g, color.b, 1f - Mathf.Clamp01(t / bajar));
            yield return null;
        }
        flashImg.color = new Color(color.r, color.g, color.b, 0f);
    }

    // Tarjeta con el dato curioso. Se cierra sola o al tocar "Continuar".
    public IEnumerator MostrarDato(string producto, string texto, float segundos)
    {
        if (string.IsNullOrEmpty(texto)) yield break;

        datoTitulo.text = "DATO CURIOSO: " + producto.ToUpperInvariant();
        datoTexto.text = texto;
        cerrarDato = false;
        datoRoot.SetActive(true);

        float t = 0f;
        while (t < 0.25f)
        {
            t += Time.unscaledDeltaTime;
            datoGroup.alpha = Mathf.Clamp01(t / 0.25f);
            yield return null;
        }
        datoGroup.alpha = 1f;

        t = 0f;
        while (!cerrarDato && t < segundos)
        {
            t += Time.unscaledDeltaTime;
            datoFill.anchorMax = new Vector2(Mathf.Clamp01(1f - t / segundos), 1f);
            yield return null;
        }

        t = 0f;
        while (t < 0.25f)
        {
            t += Time.unscaledDeltaTime;
            datoGroup.alpha = 1f - Mathf.Clamp01(t / 0.25f);
            yield return null;
        }
        datoRoot.SetActive(false);
    }

    public void MostrarResultado(string titulo, int estrellas, string detalle,
                                 Action reintentar, Action seleccion, Action menu)
    {
        resTitulo.text = titulo;
        resDetalle.text = detalle;

        // Botones (se reconectan cada vez)
        Button[] botones = resRoot.GetComponentsInChildren<Button>(true);
        foreach (Button b in botones)
        {
            b.onClick.RemoveAllListeners();
            if (b.name == "BtnReintentar") b.onClick.AddListener(() => reintentar());
            else if (b.name == "BtnSeleccion") b.onClick.AddListener(() => seleccion());
            else if (b.name == "BtnMenu") b.onClick.AddListener(() => menu());
        }

        resRoot.SetActive(true);
        StartCoroutine(AnimarEstrellas(estrellas));
    }

    private IEnumerator AnimarEstrellas(int cuantas)
    {
        for (int i = 0; i < 3; i++)
        {
            estrellasImg[i].color = new Color(0.25f, 0.25f, 0.32f, 1f);
            estrellasImg[i].rectTransform.localScale = Vector3.one;
        }

        yield return new WaitForSecondsRealtime(0.4f);

        for (int i = 0; i < cuantas; i++)
        {
            estrellasImg[i].color = colorOro;
            float t = 0f;
            while (t < 0.4f)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(t / 0.4f);
                estrellasImg[i].rectTransform.localScale = Vector3.one * Mathf.Lerp(2f, 1f, k * k);
                yield return null;
            }
            estrellasImg[i].rectTransform.localScale = Vector3.one;
            yield return new WaitForSecondsRealtime(0.15f);
        }
    }

    // ================================================================== construccion
    private void Construir()
    {
        GameObject cgo = new GameObject("Nivel3Canvas", typeof(RectTransform), typeof(Canvas),
                                        typeof(CanvasScaler), typeof(GraphicRaycaster));
        cgo.transform.SetParent(transform, false);
        Canvas canvas = cgo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = ordenCanvas;
        CanvasScaler sc = cgo.GetComponent<CanvasScaler>();
        sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        sc.referenceResolution = new Vector2(1920f, 1080f);
        sc.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        sc.matchWidthOrHeight = 0.5f;
        cRT = (RectTransform)cgo.transform;

        ConstruirTableta();
        ConstruirToast();
        ConstruirDato();
        ConstruirResultado();

        // Destello de pantalla (siempre encima de todo lo anterior)
        flashImg = Img("Destello", cRT, Vector2.zero, Vector2.one, new Color(1f, 1f, 1f, 0f), null, false);
    }

    private void ConstruirTableta()
    {
        RectTransform tab = PanelBorde("Tableta", cRT, new Vector2(0.012f, 0.58f), new Vector2(0.30f, 0.985f),
                                       colorPanel, colorBorde, 5f);

        txtPedido = Txt("Pedido", tab, new Vector2(0.05f, 0.85f), new Vector2(0.95f, 0.97f), 16f, 30f,
                        FontStyles.Bold, TextAlignmentOptions.MidlineLeft, colorBorde);
        txtInstr = Txt("Instruccion", tab, new Vector2(0.05f, 0.50f), new Vector2(0.95f, 0.84f), 16f, 28f,
                       FontStyles.Normal, TextAlignmentOptions.TopLeft, Color.white);

        // Barra de precision
        zonaBarra = NuevoRect("ZonaBarra", tab, new Vector2(0.05f, 0.30f), new Vector2(0.95f, 0.46f));
        Img("Fondo", zonaBarra, Vector2.zero, Vector2.one, new Color(1f, 1f, 1f, 0.15f),
            ManualSpriteFactory.RectRedondeado(), true);
        zonaVerde = Img("ZonaVerde", zonaBarra, new Vector2(0.4f, 0f), new Vector2(0.6f, 1f),
                        new Color32(60, 210, 120, 255), null, false).rectTransform;
        Image marc = Img("Marcador", zonaBarra, new Vector2(0f, 0f), new Vector2(0f, 1f), Color.white, null, false);
        marcadorRT = marc.rectTransform;
        marcadorRT.pivot = new Vector2(0.5f, 0.5f);
        marcadorRT.sizeDelta = new Vector2(10f, 16f);
        marcadorRT.anchoredPosition = Vector2.zero;

        // Progreso del Simon
        zonaSimon = NuevoRect("ZonaSimon", tab, new Vector2(0.05f, 0.24f), new Vector2(0.95f, 0.48f));
        for (int i = 0; i < puntos.Length; i++)
        {
            Image p = Img("Punto" + i, zonaSimon, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                          Color.white, ManualSpriteFactory.Circulo(), false);
            p.rectTransform.sizeDelta = new Vector2(34f, 34f);
            puntos[i] = p;
        }
        RectTransform barraTiempo = NuevoRect("BarraTiempo", zonaSimon, new Vector2(0f, 0f), new Vector2(1f, 0.16f));
        Img("Fondo", barraTiempo, Vector2.zero, Vector2.one, new Color(1f, 1f, 1f, 0.15f), null, false);
        timerFill = Img("Relleno", barraTiempo, Vector2.zero, Vector2.one, new Color32(255, 150, 70, 255), null, false).rectTransform;
        zonaSimon.gameObject.SetActive(false);
        zonaBarra.gameObject.SetActive(false);

        // Fusibles
        for (int i = 0; i < fusiblesImg.Length; i++)
        {
            Image f = Img("Fusible" + i, tab, new Vector2(0.05f + i * 0.095f, 0.07f),
                          new Vector2(0.05f + i * 0.095f + 0.08f, 0.07f + 0.16f), colorOro,
                          ManualSpriteFactory.RectRedondeado(), true);
            fusiblesImg[i] = f;
        }

        // Boton repetir
        btnRepetir = Boton("BtnRepetir", "Ver otra vez", tab, new Vector2(0.58f, 0.05f), new Vector2(0.95f, 0.24f),
                           new Color32(70, 140, 255, 255), 18f, 30f);
        btnRepetir.onClick.AddListener(() => { if (AlPedirRepetir != null) AlPedirRepetir(); });
        btnRepetir.gameObject.SetActive(false);
    }

    private void ConstruirToast()
    {
        RectTransform r = NuevoRect("Toast", cRT, new Vector2(0.15f, 0.56f), new Vector2(0.85f, 0.74f));
        toastGroup = r.gameObject.AddComponent<CanvasGroup>();
        toastGroup.alpha = 0f;
        toastGroup.blocksRaycasts = false;
        toastTxt = Txt("Texto", r, Vector2.zero, Vector2.one, 40f, 110f, FontStyles.Bold,
                       TextAlignmentOptions.Center, Color.white);
        toastTxt.outlineWidth = 0.28f;
        toastTxt.outlineColor = new Color32(10, 10, 30, 255);
    }

    private void ConstruirDato()
    {
        RectTransform inner = PanelBorde("Dato", cRT, new Vector2(0.27f, 0.04f), new Vector2(0.73f, 0.33f),
                                         colorPanel, colorBorde, 5f);
        datoRoot = inner.parent.gameObject;
        datoGroup = datoRoot.AddComponent<CanvasGroup>();

        datoTitulo = Txt("Titulo", inner, new Vector2(0.05f, 0.78f), new Vector2(0.95f, 0.96f), 18f, 34f,
                         FontStyles.Bold, TextAlignmentOptions.MidlineLeft, colorBorde);
        datoTexto = Txt("Texto", inner, new Vector2(0.05f, 0.30f), new Vector2(0.95f, 0.78f), 16f, 32f,
                        FontStyles.Normal, TextAlignmentOptions.TopLeft, Color.white);

        RectTransform barra = NuevoRect("Barra", inner, new Vector2(0.05f, 0.06f), new Vector2(0.60f, 0.12f));
        Img("Fondo", barra, Vector2.zero, Vector2.one, new Color(1f, 1f, 1f, 0.15f), null, false);
        datoFill = Img("Relleno", barra, Vector2.zero, Vector2.one, colorBorde, null, false).rectTransform;

        btnContinuar = Boton("BtnContinuar", "Continuar", inner, new Vector2(0.66f, 0.04f), new Vector2(0.95f, 0.24f),
                             colorBorde, 18f, 30f);
        btnContinuar.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(24, 28, 52, 255);
        btnContinuar.onClick.AddListener(() => cerrarDato = true);

        datoRoot.SetActive(false);
    }

    private void ConstruirResultado()
    {
        RectTransform rootRT = NuevoRect("Resultado", cRT, Vector2.zero, Vector2.one);
        Image dim = rootRT.gameObject.AddComponent<Image>();
        dim.color = new Color(0f, 0f, 0f, 0.75f);
        dim.raycastTarget = true;
        resRoot = rootRT.gameObject;

        RectTransform card = PanelBorde("Tarjeta", rootRT, new Vector2(0.22f, 0.12f), new Vector2(0.78f, 0.88f),
                                        colorPanel, colorBorde, 6f);

        resTitulo = Txt("Titulo", card, new Vector2(0.05f, 0.82f), new Vector2(0.95f, 0.97f), 30f, 70f,
                        FontStyles.Bold, TextAlignmentOptions.Center, Color.white);

        for (int i = 0; i < 3; i++)
        {
            float x0 = 0.12f + i * 0.28f;
            estrellasImg[i] = Img("Estrella" + i, card, new Vector2(x0, 0.46f), new Vector2(x0 + 0.24f, 0.80f),
                                  new Color(0.25f, 0.25f, 0.32f, 1f), Estrella(), false);
            estrellasImg[i].preserveAspect = true;
        }

        resDetalle = Txt("Detalle", card, new Vector2(0.06f, 0.30f), new Vector2(0.94f, 0.44f), 18f, 34f,
                         FontStyles.Normal, TextAlignmentOptions.Center, Color.white);

        Boton("BtnReintentar", "Reintentar", card, new Vector2(0.04f, 0.05f), new Vector2(0.34f, 0.24f),
              new Color32(255, 150, 70, 255), 18f, 34f);
        Boton("BtnSeleccion", "Elegir nivel", card, new Vector2(0.35f, 0.05f), new Vector2(0.65f, 0.24f),
              colorBorde, 18f, 34f);
        Boton("BtnMenu", "Menú", card, new Vector2(0.66f, 0.05f), new Vector2(0.96f, 0.24f),
              new Color32(70, 140, 255, 255), 18f, 34f);

        resRoot.SetActive(false);
    }

    // ------------------------------------------------------------------ ayudas
    private static void Anclas(RectTransform rt, Vector2 min, Vector2 max)
    {
        rt.anchorMin = min;
        rt.anchorMax = max;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static RectTransform NuevoRect(string nombre, Transform padre, Vector2 min, Vector2 max)
    {
        GameObject go = new GameObject(nombre, typeof(RectTransform));
        RectTransform rt = (RectTransform)go.transform;
        rt.SetParent(padre, false);
        Anclas(rt, min, max);
        return rt;
    }

    private static Image Img(string nombre, Transform padre, Vector2 min, Vector2 max, Color color, Sprite sprite, bool sliced)
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

    private TextMeshProUGUI Txt(string nombre, Transform padre, Vector2 min, Vector2 max, float tMin, float tMax,
                                FontStyles estilo, TextAlignmentOptions alin, Color color)
    {
        RectTransform rt = NuevoRect(nombre, padre, min, max);
        TextMeshProUGUI t = rt.gameObject.AddComponent<TextMeshProUGUI>();
        if (fuente != null) t.font = fuente;
        t.enableAutoSizing = true;
        t.fontSizeMin = tMin;
        t.fontSizeMax = tMax;
        t.fontStyle = estilo;
        t.alignment = alin;
        t.color = color;
        t.richText = true;
        t.raycastTarget = false;
        return t;
    }

    // Devuelve el panel interior; el marco (borde) es su padre
    private RectTransform PanelBorde(string nombre, Transform padre, Vector2 min, Vector2 max,
                                     Color fondo, Color borde, float grosor)
    {
        Image marco = Img(nombre + "_Marco", padre, min, max, borde, ManualSpriteFactory.RectRedondeado(), true);
        Image interior = Img(nombre, marco.transform, Vector2.zero, Vector2.one, fondo, ManualSpriteFactory.RectRedondeado(), true);
        interior.rectTransform.offsetMin = new Vector2(grosor, grosor);
        interior.rectTransform.offsetMax = new Vector2(-grosor, -grosor);
        return interior.rectTransform;
    }

    private Button Boton(string nombre, string texto, Transform padre, Vector2 min, Vector2 max,
                         Color color, float tMin, float tMax)
    {
        RectTransform rt = NuevoRect(nombre, padre, min, max);
        Image img = rt.gameObject.AddComponent<Image>();
        img.sprite = ManualSpriteFactory.RectRedondeado();
        img.type = Image.Type.Sliced;
        img.color = color;
        img.raycastTarget = true;

        Button b = rt.gameObject.AddComponent<Button>();
        b.targetGraphic = img;
        ColorBlock cb = b.colors;
        cb.highlightedColor = new Color(0.92f, 0.92f, 0.92f, 1f);
        cb.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        cb.selectedColor = Color.white;
        b.colors = cb;

        TextMeshProUGUI t = Txt("Texto", rt, Vector2.zero, Vector2.one, tMin, tMax, FontStyles.Bold,
                                TextAlignmentOptions.Center, Color.white);
        t.rectTransform.offsetMin = new Vector2(8f, 4f);
        t.rectTransform.offsetMax = new Vector2(-8f, -4f);
        t.text = texto;
        return b;
    }

    // Estrella de 5 puntas dibujada por codigo
    private static Sprite Estrella()
    {
        if (estrellaSprite != null) return estrellaSprite;

        const int n = 128;
        Vector2[] poly = new Vector2[10];
        for (int i = 0; i < 10; i++)
        {
            float ang = Mathf.PI / 2f + i * Mathf.PI / 5f;
            float r = (i % 2 == 0) ? 0.47f : 0.19f;
            poly[i] = new Vector2(0.5f + r * Mathf.Cos(ang), 0.5f + r * Mathf.Sin(ang));
        }

        Color32[] col = new Color32[n * n];
        for (int y = 0; y < n; y++)
        {
            for (int x = 0; x < n; x++)
            {
                int aciertos = 0;
                for (int sy = 0; sy < 3; sy++)
                    for (int sx = 0; sx < 3; sx++)
                    {
                        Vector2 p = new Vector2((x + (sx + 0.5f) / 3f) / n, (y + (sy + 0.5f) / 3f) / n);
                        if (Dentro(poly, p)) aciertos++;
                    }
                col[y * n + x] = new Color32(255, 255, 255, (byte)(aciertos * 255 / 9));
            }
        }

        Texture2D tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;
        tex.SetPixels32(col);
        tex.Apply();
        estrellaSprite = Sprite.Create(tex, new Rect(0, 0, n, n), new Vector2(0.5f, 0.5f), 100f);
        return estrellaSprite;
    }

    private static bool Dentro(Vector2[] poly, Vector2 p)
    {
        bool dentro = false;
        for (int i = 0, j = poly.Length - 1; i < poly.Length; j = i++)
        {
            if (((poly[i].y > p.y) != (poly[j].y > p.y)) &&
                (p.x < (poly[j].x - poly[i].x) * (p.y - poly[i].y) / (poly[j].y - poly[i].y) + poly[i].x))
                dentro = !dentro;
        }
        return dentro;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// Un pedido de la fabrica. Todos estos valores se pueden ajustar en el Inspector.
[Serializable]
public class Pedido
{
    public string producto = "Botella";
    public TipoProducto tipo = TipoProducto.Botella;
    [Range(2, 8)] public int pasos = 3;
    [Range(3, 6)] public int colores = 4;
    [Tooltip("Ancho de la zona verde de la palanca (0 a 1). Mas chico = mas dificil.")]
    [Range(0.05f, 0.5f)] public float anchoZonaVerde = 0.25f;
    public float velocidadBarra = 0.8f;
    public float velocidadCinta = 2.5f;
    [Tooltip("1 = normal. Mas alto = la secuencia se muestra mas rapido.")]
    public float velocidadSecuencia = 1f;
    public float segundosPorPaso = 6f;
    [TextArea(2, 4)] public string datoCurioso;
}

// Director del Nivel 3. Cada pedido recorre:
//   1) Palanca con punteria  ->  2) Teletransporte  ->  3) Simon (pisar placas)  ->  4) Producto en la cinta 2
public class Nivel3Manager : MonoBehaviour
{
    [Header("Piezas de la escena")]
    public LeverMachine palanca;
    public ObjectSpawner spawnerCubos;
    public TeleportPad teleport;
    public Transform destinoSimon;
    public SimonWall simon;
    public ProductLine linea;
    public Nivel3HUD hud;

    [Header("Reglas")]
    public int fusibles = 3;
    public int numeroNivel = 3;
    public bool vibrar = true;
    public List<Pedido> pedidos = PedidosPorDefecto();

    [Header("Ambiente (el cuarto se ilumina a medida que avanzas)")]
    public Light[] lucesDelCuarto;
    public Color colorSucio = new Color(0.72f, 0.76f, 0.82f);
    public Color colorLimpio = new Color(1f, 1f, 0.95f);
    [Range(0.2f, 1f)] public float intensidadInicial = 0.6f;
    public bool tenirLuces = true;

    [Header("Escenas (deben estar en Build Settings)")]
    public string escenaDerrota = "Derrota";
    public string escenaSeleccion = "LevelSelector";
    public string escenaMenu = "Menu";

    [Header("Sonidos opcionales (si no, se generan solos)")]
    public AudioClip sonidoExito;
    public AudioClip sonidoError;
    public AudioClip sonidoTeleport;
    public AudioClip sonidoDing;

    private enum Resultado { Esperando, Correcto, Error, Tiempo, Repetir }

    private AudioSource sfx;
    private Light luzGuia;
    private Transform objetivoGuia;
    private float[] intensidadesBase;
    private float progresoLuz;

    private int fusiblesRestantes;
    private readonly List<float> calidades = new List<float>();
    private bool derrotado;

    private Vector3 posRetorno;
    private float yawRetorno;

    // estado de la ronda de entrada
    private Resultado resultado;
    private List<int> secuenciaActual;
    private int indiceEntrada;
    private bool puedeRepetir;

    // ================================================================== arranque
    private void Awake()
    {
        sfx = gameObject.AddComponent<AudioSource>();
        sfx.playOnAwake = false;
        sfx.spatialBlend = 0f;

        if (palanca == null) palanca = FindFirstObjectByType<LeverMachine>();
        if (spawnerCubos == null) spawnerCubos = FindFirstObjectByType<ObjectSpawner>();
        if (teleport == null) teleport = FindFirstObjectByType<TeleportPad>();
        if (simon == null) simon = FindFirstObjectByType<SimonWall>();
        if (linea == null) linea = FindFirstObjectByType<ProductLine>();
        if (hud == null) hud = FindFirstObjectByType<Nivel3HUD>();
        if (hud == null) hud = gameObject.AddComponent<Nivel3HUD>();

        GameObject g = new GameObject("LuzGuia");
        luzGuia = g.AddComponent<Light>();
        luzGuia.type = LightType.Point;
        luzGuia.range = 5f;
        luzGuia.color = new Color(0.4f, 1f, 0.85f);
        luzGuia.intensity = 0f;

        GuardarLuces();
    }

    private void Start()
    {
        string faltan = "";
        if (palanca == null) faltan += " LeverMachine";
        if (spawnerCubos == null) faltan += " ObjectSpawner";
        if (teleport == null) faltan += " TeleportPad";
        if (simon == null) faltan += " SimonWall";
        if (linea == null) faltan += " ProductLine";
        if (pedidos == null || pedidos.Count == 0) faltan += " Pedidos";
        if (faltan.Length > 0)
        {
            Debug.LogError("Nivel3Manager: faltan referencias:" + faltan + ". Revisa Grox > Nivel 3 > 1.");
            enabled = false;
            return;
        }

        fusiblesRestantes = fusibles;
        palanca.Bloqueada = true;
        StartCoroutine(RutaNivel());
    }

    private void Update()
    {
        // Luz guia: late sobre el objeto que el jugador debe usar
        if (luzGuia == null) return;
        if (objetivoGuia != null)
        {
            luzGuia.transform.position = objetivoGuia.position + Vector3.up * 1.0f;
            luzGuia.intensity = 1.5f + 1.5f * (0.5f + 0.5f * Mathf.Sin(Time.time * 4f));
        }
        else
        {
            luzGuia.intensity = 0f;
        }
    }

    private static List<Pedido> PedidosPorDefecto()
    {
        return new List<Pedido>
        {
            new Pedido
            {
                producto = "Botella", tipo = TipoProducto.Botella, pasos = 3, colores = 4,
                anchoZonaVerde = 0.26f, velocidadBarra = 0.8f, velocidadCinta = 2.5f,
                velocidadSecuencia = 1f, segundosPorPaso = 6f,
                datoCurioso = "Las botellas de plástico recicladas pueden convertirse en fibras para hacer ropa, mochilas y hasta nuevas botellas."
            },
            new Pedido
            {
                producto = "Bolsa", tipo = TipoProducto.Bolsa, pasos = 4, colores = 5,
                anchoZonaVerde = 0.19f, velocidadBarra = 1.1f, velocidadCinta = 3.2f,
                velocidadSecuencia = 1.2f, segundosPorPaso = 5f,
                datoCurioso = "Con plástico reciclado también se fabrican bolsas y \"madera plástica\" para bancos, postes y juegos de plaza."
            },
            new Pedido
            {
                producto = "Remera", tipo = TipoProducto.Remera, pasos = 5, colores = 6,
                anchoZonaVerde = 0.13f, velocidadBarra = 1.5f, velocidadCinta = 4f,
                velocidadSecuencia = 1.4f, segundosPorPaso = 4.5f,
                datoCurioso = "Muchas prendas deportivas y de abrigo se hacen con fibras de botellas recicladas. ¡Una botella puede volver a vestirte!"
            }
        };
    }

    // ================================================================== recorrido del nivel
    private IEnumerator RutaNivel()
    {
        // Si el manual se abre al empezar, pausa el juego y este tiempo no corre hasta que se cierre
        yield return null;
        yield return new WaitForSeconds(0.5f);

        AplicarLuces(0f);
        simon.Preparar(pedidos[0].colores);
        hud.SetFusibles(fusiblesRestantes, fusibles);

        for (int i = 0; i < pedidos.Count; i++)
        {
            yield return RutaPedido(i);
            if (derrotado) yield break;
        }

        yield return Victoria();
    }

    private IEnumerator RutaPedido(int i)
    {
        Pedido p = pedidos[i];
        hud.SetPedido(i + 1, pedidos.Count, p.producto);
        hud.OcultarSimon();
        simon.Preparar(p.colores);

        // ---------------- FASE 1: palanca con punteria
        spawnerCubos.SetSpeed(p.velocidadCinta);
        while (palanca.EstaEnMarcha) yield return null;
        palanca.Bloqueada = false;

        Guiar(palanca.transform);
        hud.SetInstruccion("1. Apunta a la palanca y toca Interactuar cuando el marcador esté en la zona verde.");
        hud.MostrarBarra(UnityEngine.Random.Range(0.3f, 0.7f), p.anchoZonaVerde, p.velocidadBarra);

        bool jalada = false;
        float distancia = 1f;
        Action alJalar = () => { distancia = hud.DistanciaAlCentro; jalada = true; };
        palanca.OnPulled += alJalar;
        yield return new WaitUntil(() => jalada);
        palanca.OnPulled -= alJalar;

        palanca.Bloqueada = true;
        hud.OcultarBarra();
        Guiar(null);
        EvaluarPresion(distancia, p.anchoZonaVerde);

        hud.SetInstruccion("La prensa está compactando el plástico...");
        bool cuboListo = false;
        UnityAction alCubo = () => cuboListo = true;
        palanca.onMinigameReady.AddListener(alCubo);
        yield return new WaitUntil(() => cuboListo);
        palanca.onMinigameReady.RemoveListener(alCubo);
        yield return TragarCubo();

        // ---------------- FASE 2: teletransporte
        hud.SetInstruccion("2. Pisa el teletransportador brillante para ir a la sala de moldeo.");
        Guiar(teleport.transform);
        bool pisado = false;
        Action alPisar = () => pisado = true;
        teleport.AlPisar += alPisar;
        teleport.Activar(true);
        yield return new WaitUntil(() => pisado);
        teleport.AlPisar -= alPisar;
        Guiar(null);
        yield return Teletransportar(true);

        // ---------------- FASE 3: Simon
        hud.MostrarSimon(p.pasos);
        List<int> secuencia = Generar(p.pasos, p.colores);
        bool repetirUsado = false;
        bool completada = false;

        while (!completada)
        {
            hud.ResetPasos();
            hud.MostrarBotonRepetir(false);
            hud.SetInstruccion("3. Memoriza la secuencia de la pared.");
            yield return new WaitForSeconds(0.9f);
            yield return simon.Reproducir(secuencia, p.velocidadSecuencia);

            hud.SetInstruccion("¡Tu turno! Pisa las placas del piso en el mismo orden.");
            hud.MostrarBotonRepetir(!repetirUsado);
            yield return RondaEntrada(secuencia, p.segundosPorPaso * p.pasos, !repetirUsado);
            hud.MostrarBotonRepetir(false);

            if (resultado == Resultado.Correcto)
            {
                completada = true;
            }
            else if (resultado == Resultado.Repetir)
            {
                repetirUsado = true;
            }
            else
            {
                yield return PerderFusible(resultado == Resultado.Tiempo ? "¡Se acabó el tiempo!" : "¡Placa equivocada!");
                if (derrotado) yield break;
            }
        }

        Sfx(sonidoExito, ProceduralSfx.Exito());
        hud.Toast("¡Receta perfecta!", new Color32(255, 214, 10, 255));
        yield return simon.Celebrar();
        hud.OcultarSimon();
        yield return new WaitForSeconds(0.3f);
        yield return Teletransportar(false);
        simon.Reposo();

        // ---------------- FASE 4: producto fabricado
        hud.SetInstruccion("¡La máquina está fabricando tu " + p.producto.ToLowerInvariant() + "!");
        yield return linea.Fabricar(p.tipo, () => Sfx(sonidoDing, ProceduralSfx.Ding()));
        StartCoroutine(AnimarLuces((i + 1f) / pedidos.Count));
        yield return hud.MostrarDato(p.producto, p.datoCurioso, 6f);
    }

    // ================================================================== piezas de cada fase
    private void EvaluarPresion(float distancia, float ancho)
    {
        if (distancia <= ancho * 0.5f)
        {
            calidades.Add(1f);
            hud.Toast("¡PERFECTO!", new Color32(255, 214, 10, 255));
            Sfx(sonidoDing, ProceduralSfx.Ding());
        }
        else if (distancia <= ancho * 1.1f)
        {
            calidades.Add(0.6f);
            hud.Toast("¡Bien!", new Color32(60, 210, 120, 255));
        }
        else
        {
            calidades.Add(0.25f);
            hud.Toast("Puede mejorar", new Color32(255, 150, 70, 255));
        }
    }

    private IEnumerator TragarCubo()
    {
        GameObject cubo = spawnerCubos.Current;
        if (cubo == null) yield break;

        Vector3 escala = cubo.transform.localScale;
        Efectos.Chispas(cubo.transform.position, new Color(0.4f, 1f, 0.9f));
        Sfx(sonidoDing, ProceduralSfx.Ding());

        float t = 0f;
        while (t < 0.35f && cubo != null)
        {
            t += Time.deltaTime;
            cubo.transform.localScale = escala * (1f - t / 0.35f);
            yield return null;
        }
        if (cubo != null) Destroy(cubo);   // al destruirse, la palanca se reinicia sola
    }

    private IEnumerator Teletransportar(bool ida)
    {
        Transform raiz = JugadorN3.Raiz;
        Sfx(sonidoTeleport, ProceduralSfx.Whoosh());

        Action mover = () =>
        {
            if (raiz == null) return;
            if (ida)
            {
                posRetorno = raiz.position;
                yawRetorno = raiz.eulerAngles.y;
                if (destinoSimon != null)
                    JugadorN3.Mover(raiz, destinoSimon.position, destinoSimon.eulerAngles.y);
            }
            else
            {
                JugadorN3.Mover(raiz, posRetorno, yawRetorno);
            }
        };

        yield return hud.Pantallazo(new Color(0.8f, 1f, 1f), 0.18f, 0.45f, mover);
    }

    private List<int> Generar(int pasos, int colores)
    {
        List<int> s = new List<int>();
        int anterior = -1;
        for (int i = 0; i < pasos; i++)
        {
            int c;
            do { c = UnityEngine.Random.Range(0, colores); } while (c == anterior);   // nunca el mismo seguido
            s.Add(c);
            anterior = c;
        }
        return s;
    }

    private IEnumerator RondaEntrada(List<int> secuencia, float segundos, bool permitirRepetir)
    {
        resultado = Resultado.Esperando;
        secuenciaActual = secuencia;
        indiceEntrada = 0;
        puedeRepetir = permitirRepetir;

        simon.AlPisarColor += AlPisarColor;
        hud.AlPedirRepetir += AlPedirRepetir;
        simon.IniciarEntrada();

        float restante = segundos;
        while (resultado == Resultado.Esperando)
        {
            restante -= Time.deltaTime;
            hud.SetTiempo(restante / segundos);
            if (restante <= 0f) resultado = Resultado.Tiempo;
            yield return null;
        }

        simon.DetenerEntrada();
        simon.AlPisarColor -= AlPisarColor;
        hud.AlPedirRepetir -= AlPedirRepetir;
    }

    private void AlPisarColor(int color)
    {
        if (resultado != Resultado.Esperando) return;

        if (color == secuenciaActual[indiceEntrada])
        {
            hud.MarcarPaso(indiceEntrada, simon.colores[color]);
            indiceEntrada++;
            if (indiceEntrada >= secuenciaActual.Count) resultado = Resultado.Correcto;
        }
        else
        {
            resultado = Resultado.Error;
        }
    }

    private void AlPedirRepetir()
    {
        if (resultado == Resultado.Esperando && puedeRepetir) resultado = Resultado.Repetir;
    }

    private IEnumerator PerderFusible(string motivo)
    {
        fusiblesRestantes--;
        hud.SetFusibles(fusiblesRestantes, fusibles);
        Sfx(sonidoError, ProceduralSfx.Error());
        Vibrar();
        hud.Toast(motivo, new Color32(255, 90, 90, 255));
        Efectos.Chispas(simon.CentroPared(), new Color(1f, 0.6f, 0.2f), 40);
        StartCoroutine(hud.Pantallazo(new Color(1f, 0.1f, 0.1f), 0.08f, 0.35f, null));

        yield return simon.ParpadeoError();

        if (fusiblesRestantes <= 0)
        {
            yield return Derrota();
        }
        else
        {
            yield return new WaitForSeconds(0.6f);
        }
    }

    private IEnumerator Derrota()
    {
        derrotado = true;
        hud.Toast("¡Sin fusibles!", new Color32(255, 90, 90, 255));
        yield return new WaitForSeconds(1.6f);

        // Misma convencion que el resto del juego: la pantalla de derrota sabe a que nivel volver
        PlayerPrefs.SetString("EscenaAnterior", SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaDerrota);
    }

    private IEnumerator Victoria()
    {
        hud.SetInstruccion("¡Todos los pedidos entregados!");
        Sfx(sonidoExito, ProceduralSfx.Exito());
        yield return new WaitForSeconds(0.8f);

        int errores = fusibles - fusiblesRestantes;
        float suma = 0f;
        foreach (float c in calidades) suma += c;
        float precision = calidades.Count > 0 ? suma / calidades.Count : 1f;

        int estrellas = (errores == 0 && precision >= 0.6f) ? 3 : (errores <= 1 ? 2 : 1);
        LevelStars.Save(numeroNivel, estrellas);

        string detalle = "Fusibles: " + fusiblesRestantes + "/" + fusibles +
                         "     Precisión de la prensa: " + Mathf.RoundToInt(precision * 100f) + "%\n" +
                         "Nada se desperdicia: ¡eso es economía circular!";

        hud.MostrarResultado("¡Planta en marcha!", estrellas, detalle,
            () => { Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); },
            () => { Time.timeScale = 1f; SceneManager.LoadScene(escenaSeleccion); },
            () => { Time.timeScale = 1f; SceneManager.LoadScene(escenaMenu); });
    }

    // ================================================================== ambiente, guia y sonido
    private void Guiar(Transform objetivo)
    {
        objetivoGuia = objetivo;
    }

    private void Sfx(AudioClip propio, AudioClip generado)
    {
        AudioClip c = propio != null ? propio : generado;
        if (c != null) sfx.PlayOneShot(c);
    }

    private void Vibrar()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (vibrar) Handheld.Vibrate();
#endif
    }

    private void GuardarLuces()
    {
        if (lucesDelCuarto == null) return;
        intensidadesBase = new float[lucesDelCuarto.Length];
        for (int i = 0; i < lucesDelCuarto.Length; i++)
            intensidadesBase[i] = lucesDelCuarto[i] != null ? lucesDelCuarto[i].intensity : 0f;
    }

    // progreso 0 = cuarto apagado/gris, 1 = cuarto limpio y brillante
    private void AplicarLuces(float progreso)
    {
        progresoLuz = progreso;
        if (lucesDelCuarto == null || intensidadesBase == null) return;

        float k = Mathf.Lerp(intensidadInicial, 1f, progreso);
        Color col = Color.Lerp(colorSucio, colorLimpio, progreso);
        for (int i = 0; i < lucesDelCuarto.Length; i++)
        {
            Light l = lucesDelCuarto[i];
            if (l == null) continue;
            l.intensity = intensidadesBase[i] * k;
            if (tenirLuces) l.color = col;
        }
    }

    private IEnumerator AnimarLuces(float hacia)
    {
        float desde = progresoLuz;
        float t = 0f;
        while (t < 1.5f)
        {
            t += Time.deltaTime;
            AplicarLuces(Mathf.Lerp(desde, hacia, Mathf.SmoothStep(0f, 1f, t / 1.5f)));
            yield return null;
        }
        AplicarLuces(hacia);
    }
}

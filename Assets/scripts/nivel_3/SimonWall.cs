using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Pared de colores (Simon) + placas del piso.
//  - Los cuadros de la pared muestran la secuencia (luz + tono).
//  - El jugador la repite PISANDO las placas del piso del mismo color.
public class SimonWall : MonoBehaviour
{
    [Header("Elementos de la escena (hasta 6)")]
    public Renderer[] cuadrosPared = new Renderer[6];
    public Transform[] placas = new Transform[6];

    [Header("Colores y pasos del reciclaje")]
    public Color[] colores = new Color[]
    {
        new Color32(240, 70, 80, 255),    // rojo
        new Color32(70, 140, 255, 255),   // azul
        new Color32(60, 210, 120, 255),   // verde
        new Color32(255, 215, 90, 255),   // amarillo
        new Color32(170, 100, 230, 255),  // violeta
        new Color32(255, 150, 70, 255)    // naranja
    };
    public string[] nombresPasos = { "LAVAR", "TRITURAR", "FUNDIR", "MOLDEAR", "ENFRIAR", "EMPACAR" };
    public bool mostrarEtiquetas = true;
    public float alturaEtiqueta = 1.6f;

    [Header("Entrada")]
    [Tooltip("Segundos que hay que quedarse parado en la placa para que cuente (evita pisadas accidentales).")]
    public float segundosParaConfirmar = 0.3f;

    [Header("Sonidos opcionales (si no, se generan solos)")]
    public AudioClip[] notasPersonalizadas;

    public event Action<int> AlPisarColor;

    private const float KReposoPared = 0.18f;
    private const float KReposoPlaca = 0.45f;

    private Renderer[] rendPlacas;
    private TextMeshPro[] etiquetas;
    private AudioSource audioSrc;
    private int activos = 4;
    private bool entrada;
    private int dwellIdx = -1;
    private float dwellT;
    private bool registrado;

    private void Awake()
    {
        audioSrc = gameObject.AddComponent<AudioSource>();
        audioSrc.playOnAwake = false;
        audioSrc.spatialBlend = 0f;

        rendPlacas = new Renderer[placas.Length];
        etiquetas = new TextMeshPro[placas.Length];
        for (int i = 0; i < placas.Length; i++)
        {
            if (placas[i] == null) continue;
            rendPlacas[i] = placas[i].GetComponentInChildren<Renderer>();
            etiquetas[i] = CrearEtiqueta(i);
        }
    }

    private TextMeshPro CrearEtiqueta(int i)
    {
        GameObject go = new GameObject("Etiqueta_" + i);
        go.transform.SetParent(transform, false);
        go.transform.position = placas[i].position + Vector3.up * alturaEtiqueta;

        TextMeshPro t = go.AddComponent<TextMeshPro>();
        t.text = i < nombresPasos.Length ? nombresPasos[i] : "";
        t.fontSize = 3.2f;
        t.fontStyle = FontStyles.Bold;
        t.alignment = TextAlignmentOptions.Center;
        t.color = i < colores.Length ? colores[i] : Color.white;
        t.rectTransform.sizeDelta = new Vector2(4f, 1f);
        return t;
    }

    private void LateUpdate()
    {
        // Las etiquetas siempre miran al jugador
        Camera c = JugadorN3.Camara;
        if (c == null || etiquetas == null) return;
        for (int i = 0; i < etiquetas.Length; i++)
            if (etiquetas[i] != null && etiquetas[i].gameObject.activeSelf)
                etiquetas[i].transform.rotation = c.transform.rotation;
    }

    // ------------------------------------------------------------------ estado visual
    // Deja encendidos en reposo solo los colores que se usan en este pedido
    public void Preparar(int cantidadColores)
    {
        int max = Mathf.Min(colores.Length, Mathf.Min(placas.Length, cuadrosPared.Length));
        activos = Mathf.Clamp(cantidadColores, 1, max);
        Reposo();
    }

    public void Reposo()
    {
        for (int i = 0; i < placas.Length; i++)
        {
            bool usa = i < activos;
            if (etiquetas[i] != null) etiquetas[i].gameObject.SetActive(usa && mostrarEtiquetas);

            if (usa)
            {
                Luz(cuadrosPared, i, KReposoPared);
                LuzPlaca(i, KReposoPlaca);
            }
            else
            {
                Apagar(i);
            }
        }
    }

    private void Apagar(int i)
    {
        Color gris = new Color(0.3f, 0.3f, 0.33f);
        if (i < cuadrosPared.Length && cuadrosPared[i] != null) GlowUtil.Set(cuadrosPared[i], gris, 0f);
        if (i < rendPlacas.Length && rendPlacas[i] != null) GlowUtil.Set(rendPlacas[i], gris, 0f);
    }

    private void Luz(Renderer[] lista, int i, float k)
    {
        if (i < lista.Length && lista[i] != null) GlowUtil.Set(lista[i], colores[i], k);
    }

    private void LuzPlaca(int i, float k)
    {
        if (i < rendPlacas.Length && rendPlacas[i] != null) GlowUtil.Set(rendPlacas[i], colores[i], k);
    }

    public Vector3 CentroPared()
    {
        Vector3 suma = Vector3.zero;
        int n = 0;
        for (int i = 0; i < cuadrosPared.Length; i++)
            if (cuadrosPared[i] != null) { suma += cuadrosPared[i].transform.position; n++; }
        return n > 0 ? suma / n : transform.position;
    }

    // ------------------------------------------------------------------ reproducir la secuencia
    public IEnumerator Reproducir(List<int> secuencia, float velocidad)
    {
        entrada = false;
        float encendido = 0.55f / Mathf.Max(0.3f, velocidad);
        float pausa = 0.22f / Mathf.Max(0.3f, velocidad);

        foreach (int c in secuencia)
        {
            Luz(cuadrosPared, c, 1.6f);
            Tono(c);
            yield return new WaitForSeconds(encendido);
            Luz(cuadrosPared, c, KReposoPared);
            yield return new WaitForSeconds(pausa);
        }
    }

    // ------------------------------------------------------------------ entrada del jugador (pisar placas)
    public void IniciarEntrada()
    {
        entrada = true;
        dwellIdx = -1;
        dwellT = 0f;
        registrado = false;

        // Si ya esta parado sobre una placa, debe bajarse antes de que cuente
        int c = ColorBajoJugador();
        if (c >= 0) { dwellIdx = c; registrado = true; }
    }

    public void DetenerEntrada()
    {
        entrada = false;
        Reposo();
    }

    private void Update()
    {
        if (!entrada || Time.timeScale == 0f) return;

        int c = ColorBajoJugador();

        if (c != dwellIdx)
        {
            if (dwellIdx >= 0) LuzPlaca(dwellIdx, KReposoPlaca);   // se bajo de la placa
            dwellIdx = c;
            dwellT = 0f;
            registrado = false;
        }

        if (c < 0 || registrado) return;

        dwellT += Time.deltaTime;
        float f = Mathf.Clamp01(dwellT / Mathf.Max(0.05f, segundosParaConfirmar));
        LuzPlaca(c, Mathf.Lerp(KReposoPlaca, 1.2f, f));   // la placa se va cargando

        if (f >= 1f)
        {
            registrado = true;
            LuzPlaca(c, 1.7f);
            Tono(c);
            StartCoroutine(EcoPared(c));
            if (AlPisarColor != null) AlPisarColor(c);
        }
    }

    private IEnumerator EcoPared(int c)
    {
        Luz(cuadrosPared, c, 1.6f);
        yield return new WaitForSeconds(0.35f);
        Luz(cuadrosPared, c, KReposoPared);
    }

    private int ColorBajoJugador()
    {
        Vector3 p = JugadorN3.PosicionOjos;
        for (int i = 0; i < activos && i < placas.Length; i++)
            if (placas[i] != null && PlacaContiene(i, p)) return i;
        return -1;
    }

    private bool PlacaContiene(int i, Vector3 p)
    {
        // Usa el tamano real de la placa, sea cual sea su forma
        if (rendPlacas[i] != null)
        {
            Bounds b = rendPlacas[i].bounds;
            return b.Contains(new Vector3(p.x, b.center.y, p.z));
        }
        Vector2 d = new Vector2(p.x - placas[i].position.x, p.z - placas[i].position.z);
        return d.magnitude <= 0.7f;
    }

    // ------------------------------------------------------------------ efectos
    public IEnumerator ParpadeoError()
    {
        Color rojo = new Color(1f, 0.15f, 0.15f);
        for (int v = 0; v < 3; v++)
        {
            for (int i = 0; i < activos; i++)
                if (cuadrosPared[i] != null) GlowUtil.Set(cuadrosPared[i], rojo, 1.6f);
            yield return new WaitForSeconds(0.14f);
            for (int i = 0; i < activos; i++)
                if (cuadrosPared[i] != null) GlowUtil.Set(cuadrosPared[i], rojo, 0.1f);
            yield return new WaitForSeconds(0.1f);
        }
        Reposo();
    }

    public IEnumerator Celebrar()
    {
        for (int vuelta = 0; vuelta < 2; vuelta++)
        {
            for (int i = 0; i < activos; i++)
            {
                Luz(cuadrosPared, i, 1.8f);
                LuzPlaca(i, 1.5f);
                yield return new WaitForSeconds(0.07f);
            }
            for (int i = 0; i < activos; i++)
            {
                Luz(cuadrosPared, i, KReposoPared);
                LuzPlaca(i, KReposoPlaca);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Tono(int c)
    {
        AudioClip clip = (notasPersonalizadas != null && c < notasPersonalizadas.Length && notasPersonalizadas[c] != null)
            ? notasPersonalizadas[c]
            : ProceduralSfx.Nota(c);
        audioSrc.PlayOneShot(clip);
    }
}

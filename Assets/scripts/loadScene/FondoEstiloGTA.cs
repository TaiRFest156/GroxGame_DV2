using System.Collections;
using UnityEngine;

public class FondoEstiloGTA : MonoBehaviour
{
    [System.Serializable]
    public class SetFondo
    {
        public GameObject fondo;
        public RectTransform personaje;
        [HideInInspector] public Vector2 posInicial;
    }

    [Header("Sets (fondo + personaje)")]
    [SerializeField] private SetFondo[] sets;

    [Header("Movimiento")]
    [SerializeField] private float distanciaDerecha = 150f;
    [SerializeField] private float duracionMovimiento = 5f;

    [Header("Paneles negros")]
    [SerializeField] private CanvasGroup panelNegroFondo;
    [SerializeField] private CanvasGroup panelNegroPersonaje;

    [Header("Tiempos de cada ronda")]
    [SerializeField] private float esperaInicial = 0.5f;
    [SerializeField] private float duracionOscurecerFondo = 2.5f;
    [SerializeField, Range(0.3f, 0.9f)] private float inicioPersonaje = 0.48f;

    [Header("Cambio de set")]
    [SerializeField] private float duracionRevelado = 0.6f;

    public float DuracionTotal =>
        (esperaInicial + duracionOscurecerFondo) * 2f + duracionRevelado;

    // Tiempo que la animacion realmente ha avanzado
    private float tiempoAnim;
    private bool terminado;

    public float Progreso =>
        terminado ? 1f : Mathf.Min(0.99f, tiempoAnim / DuracionTotal);

    private void Start()
    {
        foreach (var s in sets)
            s.posInicial = s.personaje.anchoredPosition;

        StartCoroutine(Secuencia());
    }

    private IEnumerator Secuencia()
    {
        int a = Random.Range(0, sets.Length);
        int b = (a + Random.Range(1, sets.Length)) % sets.Length;

        panelNegroFondo.alpha = 0f;
        panelNegroPersonaje.alpha = 0f;
        Activar(a);
        yield return Oscurecer();

        Activar(b);
        yield return Revelar();

        yield return Oscurecer();

        terminado = true;
    }

    private void Activar(int indice)
    {
        for (int i = 0; i < sets.Length; i++)
        {
            bool activo = i == indice;
            sets[i].fondo.SetActive(activo);
            sets[i].personaje.gameObject.SetActive(activo);
            if (activo) sets[i].personaje.anchoredPosition = sets[i].posInicial;
        }
        StartCoroutine(MoverPersonaje(sets[indice]));
    }

    private IEnumerator MoverPersonaje(SetFondo s)
    {
        Vector2 inicio = s.posInicial;
        Vector2 fin = inicio + Vector2.right * distanciaDerecha;

        float t = 0f;
        while (t < duracionMovimiento)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / duracionMovimiento);
            s.personaje.anchoredPosition = Vector2.Lerp(inicio, fin, k);
            yield return null;
        }
    }

    private IEnumerator Esperar(float duracion)
    {
        float t = 0f;
        while (t < duracion)
        {
            float dt = Time.unscaledDeltaTime;
            t += dt;
            tiempoAnim += dt;
            yield return null;
        }
    }

    private IEnumerator Oscurecer()
    {
        yield return Esperar(esperaInicial);

        float inicioPj = duracionOscurecerFondo * inicioPersonaje;
        float durPj = duracionOscurecerFondo - inicioPj;

        float t = 0f;
        while (t < duracionOscurecerFondo)
        {
            float dt = Time.unscaledDeltaTime;
            t += dt;
            tiempoAnim += dt;
            panelNegroFondo.alpha = Mathf.Clamp01(t / duracionOscurecerFondo);
            panelNegroPersonaje.alpha = Mathf.Clamp01((t - inicioPj) / durPj);
            yield return null;
        }
        panelNegroFondo.alpha = 1f;
        panelNegroPersonaje.alpha = 1f;
    }

    private IEnumerator Revelar()
    {
        float t = 0f;
        while (t < duracionRevelado)
        {
            float dt = Time.unscaledDeltaTime;
            t += dt;
            tiempoAnim += dt;
            float a = 1f - Mathf.Clamp01(t / duracionRevelado);
            panelNegroFondo.alpha = a;
            panelNegroPersonaje.alpha = a;
            yield return null;
        }
        panelNegroFondo.alpha = 0f;
        panelNegroPersonaje.alpha = 0f;
    }
}
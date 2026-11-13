using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider barraProgreso;
    [SerializeField] private TMP_Text porcentajeTexto;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Fondo estilo GTA")]
    [SerializeField] private FondoEstiloGTA fondoGTA;

    [Header("Configuracion")]
    [SerializeField] private float tiempoMinimo = 3f;
    [SerializeField] private float velocidadBarra = 3f;
    [SerializeField] private float duracionFade = 0.4f;
    [SerializeField] private string escenaDePrueba = "Start_Menu";

    private IEnumerator Start()
    {
        Time.timeScale = 1f;

        // Iguala el tiempo minimo al de la animacion
        if (fondoGTA != null)
            tiempoMinimo = Mathf.Max(tiempoMinimo, fondoGTA.DuracionTotal);

        string destino = string.IsNullOrEmpty(LoadingSceneData.escenaDestino)
            ? escenaDePrueba
            : LoadingSceneData.escenaDestino;

        yield return Fade(0f, 1f);

        AsyncOperation op = SceneManager.LoadSceneAsync(destino);
        op.allowSceneActivation = false;

        float tiempo = 0f;
        float mostrado = 0f;

        while (mostrado < 1f)
        {
            tiempo += Time.unscaledDeltaTime;

            float real = Mathf.Clamp01(op.progress / 0.9f);

            // La barra sigue el progreso real de la animacion
            float porTiempo = fondoGTA != null
                ? fondoGTA.Progreso
                : Mathf.Clamp01(tiempo / tiempoMinimo);

            float objetivo = Mathf.Min(real, porTiempo);

            mostrado = Mathf.MoveTowards(mostrado, objetivo,
                velocidadBarra * Time.unscaledDeltaTime);

            barraProgreso.value = mostrado;
            porcentajeTexto.text = Mathf.RoundToInt(mostrado * 100f) + "%";

            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.2f);

        op.allowSceneActivation = true;
    }

    private IEnumerator Fade(float desde, float hasta)
    {
        float t = 0f;
        while (t < duracionFade)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(desde, hasta, t / duracionFade);
            yield return null;
        }
        canvasGroup.alpha = hasta;
    }
}
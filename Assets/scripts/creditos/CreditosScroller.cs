using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditosScroller : MonoBehaviour
{
    [SerializeField] private RectTransform texto;
    [SerializeField] private CanvasGroup overlay;
    [SerializeField] private string menuPrincipal = "MenuPrincipal";
    [SerializeField] private float velocidad = 60f;
    [SerializeField] private float multiplicadorRapido = 4f;
    [SerializeField] private float suavizado = 8f;
    [SerializeField] private float duracionAclarar = 3f;
    [SerializeField] private float duracionOscurecer = 1.5f;

    private float multiplicador = 1f;

    private IEnumerator Start()
    {
        overlay.alpha = 1f;

        // Anclaje abajo-centro y pivot arriba-centro
        texto.anchorMin = new Vector2(0.5f, 0f);
        texto.anchorMax = new Vector2(0.5f, 0f);
        texto.pivot = new Vector2(0.5f, 1f);

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(texto);

        var tmp = texto.GetComponent<TMPro.TMP_Text>();
        tmp.ForceMeshUpdate();

        float altoPantalla = ((RectTransform)transform).rect.height;
        float altoTexto = tmp.preferredHeight;

        float y = 0f;                            // texto justo debajo de la pantalla
        float yFinal = altoPantalla + altoTexto; // última línea sale por arriba
        texto.anchoredPosition = new Vector2(0f, y);

        yield return Fade(1f, 0f, duracionAclarar);

        while (y < yFinal)
        {
            bool presionando = Pointer.current != null && Pointer.current.press.isPressed;
            float objetivo = presionando ? multiplicadorRapido : 1f;
            multiplicador = Mathf.Lerp(multiplicador, objetivo, suavizado * Time.unscaledDeltaTime);

            y += velocidad * multiplicador * Time.unscaledDeltaTime;
            texto.anchoredPosition = new Vector2(0f, y);
            yield return null;
        }

        yield return Fade(0f, 1f, duracionOscurecer);
        SceneManager.LoadScene(menuPrincipal);
    }

    private IEnumerator Fade(float desde, float hasta, float duracion)
    {
        float t = 0f;
        while (t < duracion)
        {
            t += Time.unscaledDeltaTime;
            overlay.alpha = Mathf.Lerp(desde, hasta, t / duracion);
            yield return null;
        }
        overlay.alpha = hasta;
    }
    public void Salir()
    {
        SceneManager.LoadScene(menuPrincipal);
    }
}
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class ReciclajeUI : MonoBehaviour
{
    public static ReciclajeUI Instance { get; private set; }

    [Header("Referencias UI")]
    public RectTransform panelResultados;
    public TextMeshProUGUI textoResultados;
    public TextMeshProUGUI textoPuntajeFinal;
    public float tiempoAnimacionPuntaje = 2f;
    public PanelMover panelMover;
    // necesario para Button

[Header("Botón de pausa")]
public Button botonPause;

private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (panelResultados != null)
            panelResultados.anchoredPosition = new Vector2(0, -1038);
    }

    public void MostrarResultados()
    {
        // --- LUGAR 1: Sonido de Final de Partida/Éxito ---
        // Este sonido se reproduce inmediatamente al llamar a MostrarResultados().
        if (AudioManager.Instance != null)
        {
            // Se recomienda usar un nuevo clip para "Victoria" o "Resultados Finales"
            // pero usaremos 'correctTrashSound' por ahora como se indicó antes.
            AudioManager.Instance.PlaySFX(AudioManager.Instance.scoreSound);
        }
        // --------------------------------------------------

        int puntajeFinal = ReciclajeContador.CalcularPuntajeTotal();

        if (panelMover != null)
        {
            panelMover.OnMovimientoTerminado = () =>
            {
                // Mostrar resumen de objetos recogidos
                textoResultados.text = "Objetos recogidos:\n";
                foreach (var par in ReciclajeContador.conteo)
                {
                    textoResultados.text += $"{par.Key}: {par.Value}\n";
                }

                // Iniciar animación del puntaje
                StartCoroutine(AnimarPuntaje(puntajeFinal));
            };

            // El panel comienza a moverse después de que el sonido inicial ha sonado.
            panelMover.ActivarMovimiento();
            if (botonPause != null)
                botonPause.interactable = false;
        }
    }

    private IEnumerator AnimarPuntaje(int puntajeReal)
    {
        float tiempo = 0f;
        while (tiempo < tiempoAnimacionPuntaje)
        {
            int falso = Random.Range(10000, 99999);
            textoPuntajeFinal.text = $"Puntaje total: {falso}";
            yield return new WaitForSecondsRealtime(0.05f);
            tiempo += 0.05f;
        }

        textoPuntajeFinal.text = $"Puntaje total: {puntajeReal}";
    }
}

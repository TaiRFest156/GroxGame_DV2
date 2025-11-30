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

using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [Header("UI Final")]
    public GameObject panelResultados;
    public PanelMover panelMover;

    [Header("Puerta bloqueada")]
    public GameObject puertaBloqueada;

    private void Start()
    {
        // El panel de resultados se mantiene como lo tengas configurado.
    }

    // Se llama cuando termina el spawner
    public void OnSpawnerFinished()
    {
        // Mostrar resultados
        if (ReciclajeUI.Instance != null)
        {
            ReciclajeUI.Instance.MostrarResultados();
        }

        // Pausar el juego
        Time.timeScale = 0f;

        // Activar el movimiento del panel
        if (panelMover != null)
        {
            panelMover.ActivarMovimiento();
        }
    }

    // Se llama al pulsar el botón Next
    public void CargarSiguienteEscena()
    {
        // Ocultar el panel de resultados
        if (panelResultados != null)
        {
            panelResultados.SetActive(false);
        }

        // Desbloquear la puerta
        if (puertaBloqueada != null)
        {
            puertaBloqueada.SetActive(false);
        }

        // Reanudar el juego
        Time.timeScale = 1f;

        Debug.Log("Resultados ocultos. Puerta desbloqueada.");
    }
}
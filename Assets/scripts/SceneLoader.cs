using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("UI Final")]
    public GameObject panelResultados;
    public PanelMover panelMover;


    private void Start()
    {
        // Ya no desactivamos el panel al inicio
        // Si querés ocultarlo, hacelo moviéndolo fuera de la vista o con CanvasGroup
    }

    // Llamás este método cuando tu spawner termina
    public void OnSpawnerFinished()
    {
        // Mostrar resultados
        if (ReciclajeUI.Instance != null)
            ReciclajeUI.Instance.MostrarResultados();

        // Pausar juego
        Time.timeScale = 0f;

        // Activar movimiento del panel
        if (panelMover != null)
            panelMover.ActivarMovimiento();
    }

    // Método público que se conecta al botón Next
    public void CargarSiguienteEscena()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Nivel_2"); // nombre exacto de la escena
    }
}

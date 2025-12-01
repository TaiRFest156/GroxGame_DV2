using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneOfDefeat : MonoBehaviour
{
    // Método que se llama cuando el jugador presiona el botón de "Replay"
    public void OnReplayButtonPressed()
    {
        // Obtener el último nivel jugado desde PlayerPrefs y cargarlo
        int lastLevel = PlayerPrefs.GetInt("LastLevel", 1);  // Por defecto, nivel 1 si no hay nivel guardado
        SceneManager.LoadScene("Nivel_" + lastLevel);  // Cargar el último nivel jugado
    }
}

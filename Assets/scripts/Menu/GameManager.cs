using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;  // Instancia estática del GameManager
    public int currentLevel;  // Nivel actual del jugador

    void Awake()
    {
        // Asegurarnos de que solo haya una instancia de GameManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // No destruir al cambiar de escena
        }
        else
        {
            Destroy(gameObject);  // Si ya existe, destruir este objeto
        }
    }

    // Método para manejar la derrota y guardar el nivel
    public void OnLevelLost(int level)
    {
        currentLevel = level;
        PlayerPrefs.SetInt("LastLevel", level);  // Guardamos el nivel donde el jugador perdió
        SceneManager.LoadScene("SceneOfDefeat");  // Cargar la escena de derrota
    }

    // Opcional: Métodos adicionales como guardar el progreso, etc.
}

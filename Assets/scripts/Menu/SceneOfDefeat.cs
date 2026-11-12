using UnityEngine;
using UnityEngine.SceneManagement;

// Botones de fin de nivel. Sirve en la escena "Derrota" y tambien en el panel
// de victoria de cualquier nivel (Reintentar siempre recarga el nivel correcto).
public class SceneOfDefeat : MonoBehaviour
{
    [Header("Nombres de escena (deben estar en Build Settings)")]
    [SerializeField] private string defeatScene = "Derrota";
    [SerializeField] private string levelSelectScene = "LevelSelector";
    [SerializeField] private string menuScene = "Menu";
    [SerializeField] private string fallbackLevel = "Nivel_1";

    // Mismo nombre que antes: el boton "Replay" existente sigue conectado.
    public void OnReplayButtonPressed()
    {
        string current = SceneManager.GetActiveScene().name;
        string target;

        if (current != defeatScene)
        {
            // Estamos dentro de un nivel (ej. panel de victoria): recargar este nivel.
            target = current;
        }
        else
        {
            // Estamos en la escena de derrota: volver al nivel donde se perdio.
            target = PlayerPrefs.GetString("EscenaAnterior", "");
            if (string.IsNullOrEmpty(target) || target == defeatScene)
                target = fallbackLevel;
        }

        Load(target);
    }

    public void OnLevelSelectButtonPressed() => Load(levelSelectScene);

    public void OnMenuButtonPressed() => Load(menuScene);

    private void Load(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
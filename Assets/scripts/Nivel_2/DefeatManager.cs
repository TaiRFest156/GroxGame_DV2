using UnityEngine;
using UnityEngine.SceneManagement; // Para cambiar de escena

public class DefeatManager : MonoBehaviour
{
    private int consecutiveMistakes = 0;  // Para contar los errores consecutivos
    private int maxConsecutiveMistakes = 4;  // El límite de errores seguidos
    private int minScoreToLose = 900;  // Puntaje mínimo para perder

    private ResidueLevelManager levelManager;

    void Start()
    {
        // Obtener una referencia al LevelManager
        levelManager = Object.FindFirstObjectByType<ResidueLevelManager>();
    }

    // Llamar a esta función cuando el jugador comete un error
    public void PlayerMadeMistake()
    {
        consecutiveMistakes++;

        if (consecutiveMistakes >= maxConsecutiveMistakes)
        {
            TriggerDefeat();
        }
    }

    // Llamar a esta función cuando el jugador lanza el residuo correctamente
    public void PlayerCorrectThrow()
    {
        consecutiveMistakes = 0;  // Resetear los errores consecutivos
    }

    // Comprobar si el puntaje es demasiado bajo al final del tiempo
    public void CheckForTimeOutDefeat()
    {
        if (levelManager.GetScore() <= minScoreToLose)
        {
            TriggerDefeat();
        }
    }

    // Función para activar la derrota y cambiar de escena
    private void TriggerDefeat()
    {
        // Cambiar a la escena de derrota
        SceneManager.LoadScene("Derrota");
    }
}

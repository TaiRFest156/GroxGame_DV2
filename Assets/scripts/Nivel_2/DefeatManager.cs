using UnityEngine;
using UnityEngine.SceneManagement;

public class DefeatManager : MonoBehaviour
{
    private int consecutiveMistakes = 0;      // Errores consecutivos
    private int maxConsecutiveMistakes = 4;   // Limite de errores seguidos
    private int minScoreToWin = 1050;         // Debe coincidir con VictoryManager
    private bool defeatTriggered = false;     // Evita disparar la derrota dos veces

    private ResidueLevelManager levelManager;

    void Start()
    {
        levelManager = Object.FindFirstObjectByType<ResidueLevelManager>();
    }

    // Llamar cuando el jugador comete un error
    public void PlayerMadeMistake()
    {
        consecutiveMistakes++;

        if (consecutiveMistakes >= maxConsecutiveMistakes)
        {
            TriggerDefeat();
        }
    }

    // Llamar cuando el jugador acierta
    public void PlayerCorrectThrow()
    {
        consecutiveMistakes = 0;
    }

    // Al acabarse el tiempo: si no llego a la meta, pierde
    public void CheckForTimeOutDefeat()
    {
        if (levelManager != null && levelManager.GetScore() < minScoreToWin)
        {
            TriggerDefeat();
        }
    }

    private void TriggerDefeat()
    {
        if (defeatTriggered) return;
        defeatTriggered = true;

        Time.timeScale = 1f;

        // CLAVE: guardar de que nivel venimos, para que "Reintentar" vuelva aqui
        // (SpawnBasura del Nivel 1 ya hace lo mismo).
        PlayerPrefs.SetString("EscenaAnterior", SceneManager.GetActiveScene().name);

        SceneManager.LoadScene("Derrota");
    }
}
using UnityEngine;
using UnityEngine.UI;  // Necesario para interactuar con UI
using TMPro;  // Para usar TextMeshPro

public class VictoryManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject victoryPanel;  // El panel de victoria (debe estar desactivado por defecto)
    public TextMeshProUGUI scoreText;  // Texto para mostrar el puntaje
    public TextMeshProUGUI correctResiduosText;  // Texto para mostrar los residuos acertados
    public TextMeshProUGUI wrongResiduosText;  // Texto para mostrar los residuos fallados

    private int correctResiduos = 0;
    private int wrongResiduos = 0;

    private ResidueLevelManager levelManager;

    void Start()
    {
        // Buscar el LevelManager para obtener el puntaje
        levelManager = Object.FindFirstObjectByType<ResidueLevelManager>();

        // Asegurarse de que el panel de victoria esté oculto al inicio
        victoryPanel.SetActive(false);
    }

    void Update()
    {
        // Comprobar si el puntaje es 1050 o más al final del tiempo
        if (levelManager.GetScore() >= 1050 && !victoryPanel.activeSelf && levelManager.GetTimeRemaining() <= 0)
        {
            ShowVictoryPanel();
        }
    }

    // Llamado cuando un residuo es acertado
    public void OnCorrectResidue()
    {
        correctResiduos++;
    }

    // Llamado cuando un residuo es fallado
    public void OnWrongResidue()
    {
        wrongResiduos++;
    }

    // Activar el panel de victoria y mostrar los datos
    public void ShowVictoryPanel()
    {
        victoryPanel.SetActive(true);  // Mostrar el panel de victoria

        // Mostrar los datos en el panel
        scoreText.text = "Puntaje: " + levelManager.GetScore();
        correctResiduosText.text = "Residuos Aciertos: " + correctResiduos;
        wrongResiduosText.text = "Residuos Errores: " + wrongResiduos;
    }


}

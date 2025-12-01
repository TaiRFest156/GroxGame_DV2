
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelector : MonoBehaviour
{
    public GameObject levelButtonPrefab;  // Prefab del bot�n de nivel
    public Transform buttonContainer;  // Contenedor de los botones
    public int totalLevels = 2;  // N�mero total de niveles

    void Start()
    {
        GenerateLevelButtons();
    }

    // M�todo para generar los botones para cada nivel
    void GenerateLevelButtons()
    {
        for (int i = 1; i <= totalLevels; i++)
        {
            GameObject buttonObj = Instantiate(levelButtonPrefab, buttonContainer);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = "Nivel " + i;

            int levelIndex = i;

            // --- AQU� SE INSERTA LA L�GICA DE SONIDO ---
            buttonObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                // 1. Reproducir el sonido del click/bot�n
                if (AudioManager.Instance != null)
                {
                    // Usamos el AudioClip para el click
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.clickSound);
                }

                // 2. Cargar la escena (la acci�n original del bot�n)
                // Cargar el nivel seleccionado
                SceneManager.LoadScene("Nivel_" + levelIndex);
            });
            // ------------------------------------------
        }
    }

    // M�todo para regresar al men� principal
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}

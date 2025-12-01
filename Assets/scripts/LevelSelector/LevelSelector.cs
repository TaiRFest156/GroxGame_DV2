using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelector : MonoBehaviour
{
    public GameObject levelButtonPrefab;  // Prefab del botón de nivel
    public Transform buttonContainer;  // Contenedor de los botones
    public int totalLevels = 2;  // Número total de niveles

    void Start()
    {
        GenerateLevelButtons();
    }

    // Método para generar los botones para cada nivel
    void GenerateLevelButtons()
    {
        for (int i = 1; i <= totalLevels; i++)
        {
            GameObject buttonObj = Instantiate(levelButtonPrefab, buttonContainer);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = "Nivel " + i;

            int levelIndex = i;
            buttonObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                // Cargar el nivel seleccionado
                SceneManager.LoadScene("Nivel_" + levelIndex);
            });
        }
    }

    // Método para regresar al menú principal
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}

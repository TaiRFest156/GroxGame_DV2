using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelector : MonoBehaviour
{
    public GameObject levelButtonPrefab;
    public Transform buttonContainer;
    public int totalLevels = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateLevelButtons();
    }

    // --- Dentro de LevelSelector.cs ---

    void GenerateLevelButtons()
    {
        for (int i = 1; i <= totalLevels; i++)
        {
            GameObject buttonObj = Instantiate(levelButtonPrefab, buttonContainer);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = "Nivel " + i;

            int levelIndex = i;

            // --- AQUÍ SE INSERTA LA LÓGICA DE SONIDO ---
            buttonObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                // 1. Reproducir el sonido del click/botón
                if (AudioManager.Instance != null)
                {
                    // Usamos el AudioClip para el click
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.clickSound);
                }

                // 2. Cargar la escena (la acción original del botón)
                SceneManager.LoadScene("Nivel_" + levelIndex);
            });
            // ------------------------------------------
        }
    }

    // ...

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}

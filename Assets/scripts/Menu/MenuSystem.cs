using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("LevelSelector");
    }

    public void Exit()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}

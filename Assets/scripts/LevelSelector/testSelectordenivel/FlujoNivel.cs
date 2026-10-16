using UnityEngine;
using UnityEngine.SceneManagement;

public static class FlujoNivel
{
    // Se llama desde cualquier escena cuando termina
    public static void Terminar(bool gano, string siguienteEscena)
    {
        Time.timeScale = 1f;

        if (gano)
        {
            SceneManager.LoadScene(siguienteEscena);
        }
        else
        {
            PlayerPrefs.SetString("EscenaAnterior", SceneManager.GetActiveScene().name);
            SceneManager.LoadScene("Derrota");
        }
    }
}
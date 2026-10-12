
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public CanvasGroup pantallaNegra;
    public float duracion = 1.5f;

    private bool cambiando = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        pantallaNegra.alpha = 0f;
    }

    public void CambiarEscena(string nombreEscena)
    {
        if (!cambiando)
            StartCoroutine(Transicion(nombreEscena));
    }

    IEnumerator Transicion(string nombreEscena)
    {
        cambiando = true;

        // Oscurecer la escena actual
        yield return StartCoroutine(
            Fundido(0f, 1f)
        );

        // Cargar la siguiente escena
        AsyncOperation carga =
            SceneManager.LoadSceneAsync(nombreEscena);

        while (!carga.isDone)
            yield return null;

        // Mostrar la nueva escena gradualmente
        yield return StartCoroutine(
            Fundido(1f, 0f)
        );

        cambiando = false;
    }

    IEnumerator Fundido(float inicio, float fin)
    {
        float tiempo = 0f;

        pantallaNegra.alpha = inicio;

        while (tiempo < duracion)
        {
            tiempo += Time.unscaledDeltaTime;

            pantallaNegra.alpha = Mathf.Lerp(
                inicio,
                fin,
                tiempo / duracion
            );

            yield return null;
        }

        pantallaNegra.alpha = fin;
    }
}

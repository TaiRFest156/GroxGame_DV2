using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instancia;

    [Header("Pantalla negra")]
    public CanvasGroup pantallaNegra;

    [Header("Duración de la transición")]
    public float duracionOscurecer = 1.5f;
    public float duracionAclararNivel2 = 4f;

    private bool cambiando = false;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;
        DontDestroyOnLoad(gameObject);

        if (pantallaNegra != null)
        {
            pantallaNegra.alpha = 0f;
            pantallaNegra.blocksRaycasts = false;
        }
        else
        {
            Debug.LogError("Falta asignar Pantalla Negra en el Inspector.");
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Nivel_2")
        {
            StartCoroutine(AclararPantalla());
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AlCargarEscena;
    }

    private void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        if (escena.name == "Nivel_2")
        {
            StartCoroutine(AclararPantalla());
        }
    }

    public void CambiarEscena(string nombreEscena)
    {
        if (!cambiando)
        {
            StartCoroutine(Transicion(nombreEscena));
        }
    }

    private IEnumerator Transicion(string nombreEscena)
    {
        cambiando = true;

        // Oscurecer el Nivel 1.
        yield return StartCoroutine(
            Fundido(0f, 1f, duracionOscurecer)
        );

        AsyncOperation carga = SceneManager.LoadSceneAsync(nombreEscena);

        while (!carga.isDone)
        {
            yield return null;
        }

        // Para escenas distintas del Nivel 2, aclarar normalmente.
        if (nombreEscena != "Nivel_2")
        {
            yield return StartCoroutine(
                Fundido(1f, 0f, duracionOscurecer)
            );
        }

        cambiando = false;
    }

    private IEnumerator AclararPantalla()
    {
        if (pantallaNegra == null)
            yield break;

        Debug.Log("Nivel_2 detectado: aclarando pantalla.");

        pantallaNegra.alpha = 1f;
        pantallaNegra.blocksRaycasts = true;

        // Aclarar el Nivel 2 con su propia duración.
        yield return StartCoroutine(
            Fundido(1f, 0f, duracionAclararNivel2)
        );

        pantallaNegra.alpha = 0f;
        pantallaNegra.blocksRaycasts = false;

        Debug.Log("Pantalla aclarada correctamente.");
    }

    private IEnumerator Fundido(float inicio, float fin, float duracionFundido)
    {
        float tiempo = 0f;

        pantallaNegra.alpha = inicio;

        while (tiempo < duracionFundido)
        {
            tiempo += Time.unscaledDeltaTime;

            pantallaNegra.alpha = Mathf.Lerp(
                inicio,
                fin,
                Mathf.Clamp01(tiempo / duracionFundido)
            );

            yield return null;
        }

        pantallaNegra.alpha = fin;
    }
}
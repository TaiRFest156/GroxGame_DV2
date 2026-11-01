using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CasaNivel : MonoBehaviour
{
    [Header("Collider que detecta al jugador")]
    public Collider colliderTrigger;

    [Header("Jugador")]
    public string tagJugador = "Player";

    [Header("Puerta - Shape Key")]
    public SkinnedMeshRenderer puertaMesh;
    public string nombreShapeKey = "AbrirPuerta";
    public float valorPuertaAbierta = 100f;
    public float duracionPuerta = 1.5f;

    [Header("Oscurecimiento de pantalla")]
    public CanvasGroup canvasOscurecer;
    public float duracionOscurecimiento = 2.5f;

    [Header("Siguiente nivel")]
    public string siguienteNivel = "Nivel_2";

    private bool secuenciaIniciada = false;
    private int indiceShapeKey = -1;

    private void Start()
    {
        // Comprobar el Collider asignado.
        if (colliderTrigger == null)
        {
            Debug.LogError("CasaNivel: No asignaste el Collider Trigger en el Inspector.");
        }
        else if (!colliderTrigger.isTrigger)
        {
            Debug.LogWarning("CasaNivel: El Collider asignado no tiene Is Trigger activado.");
        }

        // Buscar la Shape Key.
        if (puertaMesh != null && puertaMesh.sharedMesh != null)
        {
            indiceShapeKey = puertaMesh.sharedMesh.GetBlendShapeIndex(nombreShapeKey);

            if (indiceShapeKey < 0)
            {
                Debug.LogError("CasaNivel: No se encontró la Shape Key: " + nombreShapeKey);
            }
        }
        else
        {
            Debug.LogError("CasaNivel: Asigná el SkinnedMeshRenderer de la puerta.");
        }

        // Preparar el CanvasGroup.
        if (canvasOscurecer != null)
        {
            canvasOscurecer.alpha = 0f;
            canvasOscurecer.interactable = false;
            canvasOscurecer.blocksRaycasts = false;
            canvasOscurecer.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("CasaNivel: No asignaste el CanvasGroup.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Este evento solo funciona automáticamente si el script
        // está en el objeto que recibe el evento del Collider.
        ProcesarEntrada(other);
    }

    // Método público para comprobar la entrada desde otro script.
    public void ProcesarEntrada(Collider other)
    {
        if (secuenciaIniciada)
            return;

        if (!other.CompareTag(tagJugador))
            return;

        Debug.Log("CasaNivel: ¡Jugador detectado! Iniciando secuencia.");

        secuenciaIniciada = true;
        StartCoroutine(SecuenciaEntrada());
    }

    private IEnumerator SecuenciaEntrada()
    {
        // 1. Abrir la puerta con la Shape Key.
        if (puertaMesh != null && indiceShapeKey >= 0)
        {
            float valorInicial = puertaMesh.GetBlendShapeWeight(indiceShapeKey);
            float tiempo = 0f;

            while (tiempo < duracionPuerta)
            {
                tiempo += Time.unscaledDeltaTime;

                float progreso = Mathf.Clamp01(tiempo / duracionPuerta);
                float suavizado = Mathf.SmoothStep(0f, 1f, progreso);

                float valor = Mathf.Lerp(valorInicial, valorPuertaAbierta, suavizado);

                puertaMesh.SetBlendShapeWeight(indiceShapeKey, valor);

                yield return null;
            }

            puertaMesh.SetBlendShapeWeight(indiceShapeKey, valorPuertaAbierta);
        }

        // 2. Oscurecer la pantalla.
        if (canvasOscurecer != null)
        {
            canvasOscurecer.gameObject.SetActive(true);
            canvasOscurecer.alpha = 0f;
            canvasOscurecer.blocksRaycasts = true;

            float tiempo = 0f;

            while (tiempo < duracionOscurecimiento)
            {
                tiempo += Time.unscaledDeltaTime;

                float progreso = Mathf.Clamp01(tiempo / duracionOscurecimiento);

                canvasOscurecer.alpha = Mathf.SmoothStep(0f, 1f, progreso);

                yield return null;
            }

            canvasOscurecer.alpha = 1f;
        }

        // 3. Cargar el siguiente nivel.
        Time.timeScale = 1f;
        SceneManager.LoadScene(siguienteNivel);
    }

    // Permite activar la secuencia manualmente desde otro script.
    public void ActivarSecuencia()
    {
        if (secuenciaIniciada)
            return;

        secuenciaIniciada = true;
        StartCoroutine(SecuenciaEntrada());
    }
}
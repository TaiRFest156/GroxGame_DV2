
using System.Collections;
using UnityEngine;

public class CinematicaNave : MonoBehaviour
{
    [Header("Cámaras")]
    public Camera camaraJugador;
    public Camera camaraNave;

    [Header("Seguimiento")]
    public Transform nave;

    // Cámara debajo de la nave
    public Vector3 desplazamiento = new Vector3(0f, -3f, -7f);
    public float suavidad = 3f;

    [Header("Duración")]
    public float duracionCinematica = 3f;

    private bool reproduciendo = false;

    void Start()
    {
        // Al comenzar, el jugador conserva el control de su cámara
        if (camaraJugador != null)
            camaraJugador.enabled = true;

        // La cámara cinematográfica comienza apagada
        if (camaraNave != null)
            camaraNave.enabled = false;
    }

    public void IniciarCinematica()
    {
        if (!reproduciendo)
            StartCoroutine(Secuencia());
    }

    IEnumerator Secuencia()
    {
        if (camaraJugador == null || camaraNave == null || nave == null)
        {
            Debug.LogError("Asigná las cámaras y la nave en el Inspector.");
            yield break;
        }

        reproduciendo = true;

        // Cambiar a la cámara de la nave
        camaraJugador.enabled = false;
        camaraNave.enabled = true;

        float tiempo = 0f;

        while (tiempo < duracionCinematica)
        {
            // Mantener la cámara debajo de la nave
            Vector3 posicionObjetivo = nave.position + nave.TransformDirection(desplazamiento);

            camaraNave.transform.position = Vector3.Lerp(
                camaraNave.transform.position,
                posicionObjetivo,
                suavidad * Time.deltaTime
            );

            // Mirar un punto debajo del centro para ver mejor la compuerta
            Vector3 puntoMirada = nave.position + nave.TransformDirection(
                new Vector3(0f, -1f, 0f)
            );

            camaraNave.transform.LookAt(puntoMirada);

            tiempo += Time.deltaTime;
            yield return null;
        }

        // Devolver el control de cámara al jugador
        camaraNave.enabled = false;
        camaraJugador.enabled = true;

        reproduciendo = false;
    }
}

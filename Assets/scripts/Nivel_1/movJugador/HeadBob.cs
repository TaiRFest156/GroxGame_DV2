using UnityEngine;

/// <summary>
/// Efecto de caminata: mueve la cámara arriba/abajo y a los lados según la velocidad real del jugador.
/// Ponerlo en un objeto PADRE de la cámara (ver jerarquía en las instrucciones),
/// así no choca con el script que rota la cámara (Gyro / CameraController).
/// </summary>
public class HeadBob : MonoBehaviour
{
    [Header("Referencias")]
    public CharacterController controller;   // si está vacío lo busca en los padres

    [Header("Vaivén")]
    public float frecuencia = 2.5f;          // más alto = pasos más rápidos
    public float amplitudVertical = 0.05f;   // subida y bajada
    public float amplitudLateral = 0.03f;    // balanceo izquierda/derecha
    public float suavizado = 10f;            // velocidad para volver al reposo al detenerse

    [Header("Pasos (opcional)")]
    public AudioSource fuenteAudio;
    public AudioClip[] sonidosPasos;

    private Vector3 posicionReposo;
    private float timer;
    private bool pasoReproducido;

    void Start()
    {
        posicionReposo = transform.localPosition;

        if (controller == null)
            controller = GetComponentInParent<CharacterController>();
    }

    void Update()
    {
        if (controller == null) return;

        // Solo velocidad horizontal (ignora la gravedad)
        Vector3 v = controller.velocity;
        v.y = 0f;
        float velocidad = v.magnitude;

        Vector3 objetivo = posicionReposo;

        if (controller.isGrounded && velocidad > 0.1f)
        {
            // Más velocidad (joystick más empujado) = pasos más rápidos
            timer += Time.deltaTime * velocidad * frecuencia;

            float seno = Mathf.Sin(timer * 2f); // 2 pasos por ciclo
            objetivo.y += seno * amplitudVertical;
            objetivo.x += Mathf.Cos(timer) * amplitudLateral;

            ReproducirPaso(seno);
        }
        else
        {
            pasoReproducido = false;
        }

        transform.localPosition = Vector3.Lerp(
            transform.localPosition, objetivo, Time.deltaTime * suavizado);
    }

    // Suena un paso cada vez que la cámara llega al punto más bajo
    void ReproducirPaso(float seno)
    {
        if (fuenteAudio == null || sonidosPasos == null || sonidosPasos.Length == 0) return;

        if (seno < -0.95f && !pasoReproducido)
        {
            pasoReproducido = true;
            fuenteAudio.PlayOneShot(sonidosPasos[Random.Range(0, sonidosPasos.Length)]);
        }
        else if (seno > 0f)
        {
            pasoReproducido = false;
        }
    }
}

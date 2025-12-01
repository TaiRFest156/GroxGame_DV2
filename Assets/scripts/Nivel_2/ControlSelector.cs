using UnityEngine;

public class ControlSelector : MonoBehaviour
{
    public GameObject controlPanel; // Panel con los botones
    private bool usingGyroscope = false;

    void Start()
    {
        // Mostrar el panel de opciones al iniciar
        controlPanel.SetActive(true);

        // Asegúrate de que el giroscopio esté disponible
        if (!SystemInfo.supportsGyroscope)
        {
            Debug.LogError("El dispositivo no soporta giroscopio");
        }
    }

    // Función para elegir Giroscopio
    public void SelectGyroscope()
    {
        usingGyroscope = true;
        controlPanel.SetActive(false); // Ocultar el panel de opciones
        Debug.Log("Control seleccionado: Giroscopio");
    }

    // Función para elegir Acelerómetro
    public void SelectAccelerometer()
    {
        usingGyroscope = false;
        controlPanel.SetActive(false); // Ocultar el panel de opciones
        Debug.Log("Control seleccionado: Acelerómetro");
    }

    void Update()
    {
        // Activar el control correspondiente según la selección
        if (usingGyroscope)
        {
            if (Input.gyro.enabled == false)
            {
                Input.gyro.enabled = true;
            }
            // Aquí agregar la lógica de control con giroscopio
            Vector3 gyroRotation = Input.gyro.rotationRateUnbiased;
            // Utilizar `gyroRotation` para mover el objeto o lanzar los residuos
        }
        else
        {
            // Usar el acelerómetro
            Vector3 acceleration = Input.acceleration;
            // Utilizar `acceleration` para mover el objeto o lanzar los residuos
        }
    }
}

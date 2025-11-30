using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [Header("Touch Look")]
    public float touchSensitivity = 0.15f;

    [Header("Gyro Look")]
    public float gyroSensitivity = 3f;
    public float gyroSmooth = 0.1f;

    [Header("Vertical Clamp")]
    public float minY = -80f;
    public float maxY = 80f;

    Gyroscope gyro;

    private Vector2 touchDelta;
    private float rotX;
    private float rotY;

    private Vector3 gyroAccumulated;
    private int fingerID = -1;

    void Start()
    {
        // Touch setup
#if UNITY_EDITOR
        Cursor.lockState = CursorLockMode.None;
#endif

        // Gyro setup
        gyro = Input.gyro;
        gyro.enabled = true;

        Vector3 initial = transform.localEulerAngles;
        rotX = initial.x;
        rotY = initial.y;
    }

    void Update()
    {
        HandleTouch();
        HandleGyro();

        ApplyRotation();
    }

    void HandleTouch()
    {
        touchDelta = Vector2.zero;

        if (Input.touchCount == 0)
        {
            fingerID = -1;
            return;
        }

        foreach (Touch t in Input.touches)
        {
            // Solo procesar si el toque está en la mitad derecha de la pantalla
            if (t.position.x < Screen.width / 2)
                continue;

            if (t.phase == TouchPhase.Began)
            {
                if (!EventSystem.current.IsPointerOverGameObject(t.fingerId))
                {
                    fingerID = t.fingerId;
                }
            }

            if (t.fingerId == fingerID)
            {
                if (t.phase == TouchPhase.Moved)
                {
                    touchDelta = t.deltaPosition * touchSensitivity;
                }
                else if (t.phase == TouchPhase.Ended)
                {
                    fingerID = -1;
                }
            }
        }
    }


    void HandleGyro()
    {
        Vector3 g = gyro.rotationRateUnbiased;

        // Convertir a rotación 
        Vector3 gyroRot = new Vector3(-g.x, -g.y, 0) * gyroSensitivity;

        // DEADZONE: evita que gire solo
        if (gyroRot.magnitude < 0.02f)
            gyroRot = Vector3.zero;

        // Suavizado sin acumular infinitamente
        gyroAccumulated = Vector3.Lerp(gyroAccumulated, gyroRot, gyroSmooth);
    }

    void ApplyRotation()
    {
        // Touch
        rotX -= touchDelta.y;
        rotY += touchDelta.x;

        // Gyro
        rotX += gyroAccumulated.x;
        rotY += gyroAccumulated.y;

        // Clamp vertical
        rotX = Mathf.Clamp(rotX, minY, maxY);

        transform.localRotation = Quaternion.Euler(rotX, rotY, 0);
    }
    public void SetGyroSensitivity(float value)
    {
        gyroSensitivity = value;
    }
    public float GetGyroSensitivity()
    {
        return gyroSensitivity;
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class MobileGyro : MonoBehaviour
{
    [Header("Gyro Settings")]
    public float gyroSensitivity = 8.0f;
    public float accelerometerSensitivity = 2.0f;
    public bool gyroEnabled = true;

    [Header("Aim Assistance")]
    public float smoothTime = 0.05f;
    public float maxRotationSpeed = 10.0f;
    public float deadZone = 0.01f;

    [Header("Input Mix")]
    [Range(0f, 1f)] public float gyroWeight = 0.9f;
    [Range(0f, 1f)] public float accelWeight = 0.1f;

    private Vector3 rotation;
    private Vector3 currentVelocity;
    private Quaternion initialOrientation;
    private Quaternion gyroInitialRotation;
    private bool isInitialized = false;

    void Start()
    {
        EnableGyro();
        ResetGyroReference();
    }

    void EnableGyro()
    {
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
            Input.gyro.updateInterval = 0.0167f;
        }
    }

    void ResetGyroReference()
    {
        if (SystemInfo.supportsGyroscope)
        {
            // Guardar la orientación inicial como referencia
            gyroInitialRotation = Input.gyro.attitude;
            initialOrientation = transform.rotation;
            isInitialized = true;
        }
    }

    void Update()
    {
        if (!gyroEnabled || !isInitialized) return;

        // Obtener input relativo a la orientación inicial
        Vector3 gyroInput = GetGyroRotationRate(); // Usar rotationRate en lugar de attitude
        Vector3 accelInput = GetAccelerometerInput();

        // Aplicar deadzone
        gyroInput = ApplyDeadZone(gyroInput);
        accelInput = ApplyDeadZone(accelInput);

        // Mezcla configurable
        Vector3 combinedInput = (gyroInput * gyroWeight) + (accelInput * accelWeight);

        // Solo aplicar suavizado si el input es significativo
        if (combinedInput.magnitude > deadZone)
        {
            rotation = Vector3.SmoothDamp(rotation, combinedInput,
                         ref currentVelocity, smoothTime, maxRotationSpeed);
        }
        else
        {
            rotation = Vector3.zero;
            currentVelocity = Vector3.zero;
        }

        // Aplicar rotación
        ApplyRotation(rotation);
    }

    Vector3 GetGyroRotationRate()
    {
        if (!Input.gyro.enabled) return Vector3.zero;

        // Usar rotationRateUnbiased que da valores instantáneos, no acumulativos
        return new Vector3(
            -Input.gyro.rotationRateUnbiased.x * gyroSensitivity,
            -Input.gyro.rotationRateUnbiased.y * gyroSensitivity,
            Input.gyro.rotationRateUnbiased.z * gyroSensitivity
        );
    }

    Vector3 GetAccelerometerInput()
    {
        return new Vector3(
            Input.acceleration.x * accelerometerSensitivity,
            Input.acceleration.y * accelerometerSensitivity,
            Input.acceleration.z * accelerometerSensitivity
        );
    }

    Vector3 ApplyDeadZone(Vector3 input)
    {
        input.x = Mathf.Abs(input.x) < deadZone ? 0f : input.x;
        input.y = Mathf.Abs(input.y) < deadZone ? 0f : input.y;
        input.z = Mathf.Abs(input.z) < deadZone ? 0f : input.z;
        return input;
    }

    void ApplyRotation(Vector3 rotation)
    {
        // Rotación incremental, no absoluta
        transform.Rotate(
            rotation.y * Time.deltaTime * 50f,
            rotation.x * Time.deltaTime * 50f,
            0,
            Space.World
        );
    }

    // Método para forzar reset cuando se incline permanentemente
    public void ForceResetOrientation()
    {
        ResetGyroReference();
        rotation = Vector3.zero;
        currentVelocity = Vector3.zero;

        // Opcional: resetear la rotación del objeto a cero
        transform.rotation = Quaternion.identity;
    }

    // Método para diagnóstico
    void OnGUI()
    {
        if (!gyroEnabled) return;

        GUI.Label(new Rect(10, 10, 300, 20), $"Gyro Input: {GetGyroRotationRate()}");
        GUI.Label(new Rect(10, 30, 300, 20), $"Rotation: {rotation}");
        GUI.Label(new Rect(10, 50, 300, 20), $"Device Attitude: {Input.gyro.attitude.eulerAngles}");

        if (GUI.Button(new Rect(10, 80, 200, 30), "Reset Gyro Reference"))
        {
            ForceResetOrientation();
        }
    }

    // Métodos para configuración en tiempo real
    public void SetGyroSensitivity(float sensitivity)
    {
        gyroSensitivity = sensitivity;
    }

    public void SetSmoothTime(float smooth)
    {
        smoothTime = smooth;
    }

    public void ToggleGyro(bool enabled)
    {
        gyroEnabled = enabled;
        if (!enabled)
        {
            rotation = Vector3.zero;
            currentVelocity = Vector3.zero;
        }
        else
        {
            ResetGyroReference();
        }
    }
}
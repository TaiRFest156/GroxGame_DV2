using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gyro : MonoBehaviour
{
    [Header("Configuración Gyro")]
    public float sensitivity = 0.5f; // Control de sensibilidad
    public float deadZone = 0.01f;   // Eliminar micro-movimientos

    Gyroscope m_Gyro;
    private Vector3 rotationVelocity;
    private float smoothTime = 0.1f;

    void Start()
    {
#if UNITY_EDITOR
        enabled = false; // Desactiva el script en PC
        return;
#endif
        // Configurar el gyroscopio
        if (SystemInfo.supportsGyroscope)
        {
            m_Gyro = Input.gyro;
            m_Gyro.enabled = true;
            m_Gyro.updateInterval = 0.0167f; // 60Hz
        }
    }

    void Update()
    {
        if (m_Gyro == null || !m_Gyro.enabled) return;

        // Obtener input del gyro con filtrado
        Vector3 gyroInput = GetFilteredGyroInput();

        // Aplicar rotación suavizada SOLO en X e Y
        if (gyroInput.magnitude > deadZone)
        {
            ApplyRotationWithoutZ(gyroInput * Time.deltaTime);
        }
    }

    Vector3 GetFilteredGyroInput()
    {
        Vector3 rawInput = m_Gyro.rotationRateUnbiased;

        // Aplicar deadzone para cada eje
        if (Mathf.Abs(rawInput.x) < deadZone) rawInput.x = 0;
        if (Mathf.Abs(rawInput.y) < deadZone) rawInput.y = 0;
        if (Mathf.Abs(rawInput.z) < deadZone) rawInput.z = 0;

        // Aplicar sensibilidad e invertir ejes - CERO en Z
        return new Vector3(-rawInput.x, -rawInput.y, 0) * sensitivity;
    }

    // Método para aplicar rotación SIN afectar el eje Z
    void ApplyRotationWithoutZ(Vector3 rotation)
    {
        // Obtener la rotación actual
        Vector3 currentEuler = transform.eulerAngles;

        // Aplicar solo las rotaciones en X e Y, mantener Z intacto
        transform.eulerAngles = new Vector3(
            currentEuler.x + rotation.x,
            currentEuler.y + rotation.y,
            currentEuler.z // Mantener el valor original de Z
        );
    }

    // Método alternativo más eficiente
    void ApplyRotationWithoutZAlternative(Vector3 rotation)
    {
        // Rotar solo en los ejes X e Y, ignorando Z
        transform.Rotate(rotation.x, rotation.y, 0, Space.World);
    }

    // Método para resetear la rotación (solo X e Y)
    public void ResetRotation()
    {
        // Mantener la rotación Z actual, resetear solo X e Y
        Vector3 currentEuler = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0, 0, currentEuler.z);
    }

    // Método para resetear completamente
    public void ResetCompleteRotation()
    {
        transform.rotation = Quaternion.identity;
    }

    // Método para cambiar sensibilidad
    public void SetSensitivity(float newSensitivity)
    {
        sensitivity = newSensitivity;
    }

    void OnGUI()
    {
        // Output útil para debugging
        if (m_Gyro != null && m_Gyro.enabled)
        {
            GUI.Label(new Rect(10, 10, 300, 20), "Gyro input: " + GetFilteredGyroInput());
            GUI.Label(new Rect(10, 30, 300, 20), "Sensitivity: " + sensitivity);
            GUI.Label(new Rect(10, 50, 300, 20), "Rotation: " + transform.eulerAngles);

            if (GUI.Button(new Rect(10, 80, 150, 30), "Reset Rotation"))
            {
                ResetRotation();
            }

            if (GUI.Button(new Rect(10, 120, 150, 30), "Reset Complete"))
            {
                ResetCompleteRotation();
            }
        }
    }
}
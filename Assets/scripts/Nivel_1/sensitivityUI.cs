using UnityEngine;
using UnityEngine.UI;
using TMPro; // Importante para usar TextMeshPro

public class SensitivityUI : MonoBehaviour
{
    public Slider sensitivitySlider;
    public TMP_Text sensitivityText; // Cambio aquí
    public CameraController cameraController;

    void Start()
    {
        sensitivitySlider.onValueChanged.AddListener(OnSliderChanged);
        OnSliderChanged(sensitivitySlider.value); // inicializa el texto
    }

    void OnSliderChanged(float value)
    {
        sensitivityText.text = $"Sens: {value:F2}";
        cameraController.SetGyroSensitivity(value);
    }
}

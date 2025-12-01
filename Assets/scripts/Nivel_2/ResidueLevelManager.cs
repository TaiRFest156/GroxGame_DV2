using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ResidueLevelManager : MonoBehaviour
{
    [Header("Residues")]
    public ResidueData[] residuePool;
    public Transform spawnPoint;
    public float launchSpeed = 5f;

    [Header("Target Points (3D world)")]
    public Transform targetLeft;
    public Transform targetRight;
    public Transform targetUp;

    [Header("Gameplay")]
    public float levelDuration = 90f;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    private int score = 0;
    private float timeRemaining;
    private GameObject currentResidueInstance;
    private ResidueData currentResidueData;
    private bool residueThrown = false;

    public ParticleSystem residueParticles;

    private DefeatManager defeatManager;

    private VictoryManager victoryManager;

    // Variable para saber si usar giroscopio o acelerómetro
    private bool useGyroscope = false; // Si es true, usar giroscopio. Si es false, usar acelerómetro.

    // Panel de selección de control
    public GameObject controlPanel; // Este es el panel con los botones de selección de control

    void Start()
    {
        // Congelar el tiempo al inicio del nivel
        Time.timeScale = 0f;  // Congelar el juego para esperar la selección del control

        timeRemaining = levelDuration;
        // Mostrar el panel de control (se debería activar en la UI)
        controlPanel.SetActive(true);

        // Si el giroscopio no está disponible, forzamos el uso del acelerómetro
        if (!SystemInfo.supportsGyroscope)
        {
            useGyroscope = false;
            Debug.LogWarning("Giroscopio no soportado. Usando acelerómetro.");

        }
        defeatManager = Object.FindFirstObjectByType<DefeatManager>();

        victoryManager = Object.FindFirstObjectByType<VictoryManager>();
    }

    void Update()
    {
        // Si el juego no está congelado, seguimos actualizando el tiempo y la jugabilidad
        if (Time.timeScale == 1f)
        {
            UpdateTimer();

            if (!residueThrown && currentResidueInstance != null)
            {
                // Si el giroscopio está habilitado, se usa Input.gyro. Si no, usamos Input.acceleration
                if (useGyroscope)
                    CheckGyroscope();
                else
                    CheckAccelerometer();
            }
        }
    }

    void CheckGyroscope()
    {
        // Obtén la rotación del giroscopio (puedes ajustar estos valores según tu necesidad)
        Vector3 gyroRotation = Input.gyro.rotationRateUnbiased;

        bool leftTilt = gyroRotation.x < -0.35f;
        bool rightTilt = gyroRotation.x > 0.35f;
        bool upTilt = gyroRotation.y > 0.45f;

        if (leftTilt)
            StartCoroutine(ThrowResidue(targetLeft, ResidueKind.Papel));
        else if (rightTilt)
            StartCoroutine(ThrowResidue(targetRight, ResidueKind.Plastico));
        else if (upTilt)
            StartCoroutine(ThrowResidue(targetUp, ResidueKind.Vidrio));
    }

    void CheckAccelerometer()
    {
        // Obtén los valores del acelerómetro
        Vector3 tilt = Input.acceleration;

        bool leftTilt = tilt.x < -0.35f;
        bool rightTilt = tilt.x > 0.35f;
        bool upTilt = tilt.y > 0.45f;

        if (leftTilt)
            StartCoroutine(ThrowResidue(targetLeft, ResidueKind.Papel));
        else if (rightTilt)
            StartCoroutine(ThrowResidue(targetRight, ResidueKind.Plastico));
        else if (upTilt)
            StartCoroutine(ThrowResidue(targetUp, ResidueKind.Vidrio));
    }

    IEnumerator ThrowResidue(Transform targetPoint, ResidueKind directionKind)
    {
        residueThrown = true;

        Vector3 startPos = currentResidueInstance.transform.position;
        Vector3 endPos = targetPoint.position;
        endPos.z = startPos.z;

        // Activar partículas del residuo durante el movimiento
        ParticleSystem residueParticleSystem = currentResidueInstance.GetComponent<ParticleSystem>();
        if (residueParticleSystem != null)
        {
            residueParticleSystem.Play();
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * launchSpeed;
            float smooth = Mathf.SmoothStep(0f, 1f, t);
            currentResidueInstance.transform.position = Vector3.Lerp(startPos, endPos, smooth);
            yield return null;
        }

        currentResidueInstance.transform.position = endPos;

        // Detener las partículas cuando llegue al tacho
        if (residueParticleSystem != null)
        {
            residueParticleSystem.Stop();
        }

        // Activar las partículas al llegar al tacho
        if (residueParticles != null)
        {
            residueParticles.transform.position = endPos;  // Mover las partículas al tacho
            residueParticles.Play();  // Reproducir el efecto de partículas
        }

        if (currentResidueData.kind == directionKind)
        {
            score += currentResidueData.scoreValue;
            victoryManager.OnCorrectResidue();  // Contabilizar como acierto
        }
        else
        {
            score += currentResidueData.penaltyValue;
            victoryManager.OnWrongResidue();  // Contabilizar como error
        }
        UpdateUI();

        Destroy(currentResidueInstance);
        yield return new WaitForSeconds(0.3f);

        residueThrown = false;
        SpawnResidue();
    }

    void SpawnResidue()
    {
        currentResidueData = residuePool[Random.Range(0, residuePool.Length)];
        currentResidueInstance = Instantiate(currentResidueData.prefab, spawnPoint.position, Quaternion.identity);
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Puntaje: " + score;
    }

    void UpdateTimer()
    {
        timeRemaining -= Time.deltaTime;

        if (timerText != null)
            timerText.text = Mathf.Ceil(timeRemaining).ToString();

        if (timeRemaining <= 0)
        {
            Destroy(currentResidueInstance);
            enabled = false;
        }
    }

    // Función para activar el giroscopio
    public void ActivateGyroscope()
    {
        useGyroscope = true;
        controlPanel.SetActive(false); // Ocultar el panel de control (mini fondo)
        Time.timeScale = 1f;  // Reanudar el juego
        Debug.Log("Control: Giroscopio");
    }

    // Función para activar el acelerómetro
    public void ActivateAccelerometer()
    {
        useGyroscope = false;
        controlPanel.SetActive(false); // Ocultar el panel de control (mini fondo)
        Time.timeScale = 1f;  // Reanudar el juego
        Debug.Log("Control: Acelerómetro");
    }

    public int GetScore()
    {
        return score;
    }

    public float GetTimeRemaining()
    {
        return timeRemaining;
    }
}

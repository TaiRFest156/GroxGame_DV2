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

    // Variable para saber si usar giroscopio o aceler�metro
    private bool useGyroscope = false; // Si es true, usar giroscopio. Si es false, usar aceler�metro.

    // Panel de selecci�n de control
    public GameObject controlPanel; // Este es el panel con los botones de selecci�n de control

    void Start()
    {
        // Congelar el tiempo al inicio del nivel
        Time.timeScale = 0f;  // Congelar el juego para esperar la selecci�n del control

        timeRemaining = levelDuration;
        // Mostrar el panel de control (se deber�a activar en la UI)
        controlPanel.SetActive(true);

        // Si el giroscopio no est� disponible, forzamos el uso del aceler�metro
        if (!SystemInfo.supportsGyroscope)
        {
            useGyroscope = false;
            Debug.LogWarning("Giroscopio no soportado. Usando aceler�metro.");
        }
    }

    void Update()
    {
        // Si el juego no est� congelado, seguimos actualizando el tiempo y la jugabilidad
        if (Time.timeScale == 1f)
        {
            UpdateTimer();

            if (!residueThrown && currentResidueInstance != null)
            {
                // Si el giroscopio est� habilitado, se usa Input.gyro. Si no, usamos Input.acceleration
                if (useGyroscope)
                    CheckGyroscope();
                else
                    CheckAccelerometer();
            }
        }
    }

    void CheckGyroscope()
    {
        // Obt�n la rotaci�n del giroscopio (puedes ajustar estos valores seg�n tu necesidad)
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
        // Obt�n los valores del aceler�metro
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

    

    // --- Dentro de ResidueLevelManager.cs ---

    IEnumerator ThrowResidue(Transform targetPoint, ResidueKind directionKind)
    {
        residueThrown = true;

        Vector3 startPos = currentResidueInstance.transform.position;

        // ESTA L�NEA DE DECLARACI�N FALTABA:
        Vector3 endPos = targetPoint.position;
        endPos.z = startPos.z;

        // Activar part�culas del residuo durante el movimiento
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

        // ESTA L�NEA FALTABA:
        currentResidueInstance.transform.position = endPos;

        // L�GICA DE ACIERTO/FALLO Y AUDIO (Lo que agregamos)
        bool isCorrect = currentResidueData.kind == directionKind;

        if (isCorrect)
        {
        // Detener las part�culas cuando llegue al tacho
        if (residueParticleSystem != null)
        {
            residueParticleSystem.Stop();
        }

        // Activar las part�culas al llegar al tacho
        if (residueParticles != null)
        {
            residueParticles.transform.position = endPos;  // Mover las part�culas al tacho
            residueParticles.Play();  // Reproducir el efecto de part�culas
        }

        if (currentResidueData.kind == directionKind)
            score += currentResidueData.scoreValue;

            // Sonido de ACIERTO
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.correctTrashSound);
            }
        }
        else
        {
            score += currentResidueData.penaltyValue;

            // Sonido de FALLO
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.wrongTrashSound);
            }
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

    // Funci�n para activar el giroscopio
    public void ActivateGyroscope()
    {
        useGyroscope = true;
        controlPanel.SetActive(false); // Ocultar el panel de control (mini fondo)
        Time.timeScale = 1f;  // Reanudar el juego
        Debug.Log("Control: Giroscopio");
    }

    // Funci�n para activar el aceler�metro
    public void ActivateAccelerometer()
    {
        useGyroscope = false;
        controlPanel.SetActive(false); // Ocultar el panel de control (mini fondo)
        Time.timeScale = 1f;  // Reanudar el juego
        Debug.Log("Control: Aceler�metro");
    }
}

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
    public float levelDuration = 30f;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    private int score = 0;
    private float timeRemaining;
    private GameObject currentResidueInstance;
    private ResidueData currentResidueData;
    private bool residueThrown = false;

    public ParticleSystem residueParticles;

    public VictoryManager victoryManager;

    public DefeatManager defeatManager;



    void Start()
    {
        Input.gyro.enabled = true;
        timeRemaining = levelDuration;
        SpawnResidue();
        Time.timeScale = 1f;

        victoryManager = Object.FindFirstObjectByType<VictoryManager>();
        defeatManager = Object.FindFirstObjectByType<DefeatManager>();

    }

    void Update()
    {
        UpdateTimer();

        if (!residueThrown && currentResidueInstance != null)
            CheckTilt();

        if (!residueThrown && currentResidueInstance != null)
        {
            // Detectar giroscopio
            CheckTilt();

            // Detectar teclado
            CheckKeyboardInput();
        }
    }

    void CheckKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(ThrowResidue(targetLeft, ResidueKind.Papel)); // Lanzar hacia la izquierda (A)
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(ThrowResidue(targetUp, ResidueKind.Vidrio)); // Lanzar hacia arriba (W)
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(ThrowResidue(targetRight, ResidueKind.Plastico)); // Lanzar hacia la derecha (D)
        }
    }

    void CheckTilt()
    {
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

    /*IEnumerator ThrowResidue(Transform targetPoint, ResidueKind directionKind)
    {
        residueThrown = true;

        Vector3 startPos = currentResidueInstance.transform.position;
        Vector3 endPos = targetPoint.position;
        endPos.z = startPos.z;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * launchSpeed;
            float smooth = Mathf.SmoothStep(0f, 1f, t);
            currentResidueInstance.transform.position = Vector3.Lerp(startPos, endPos, smooth);
            yield return null;
        }

        currentResidueInstance.transform.position = endPos;

        if (currentResidueData.kind == directionKind)
            score += currentResidueData.scoreValue;
        else
            score += currentResidueData.penaltyValue;

        UpdateUI();

        Destroy(currentResidueInstance);
        yield return new WaitForSeconds(0.3f);

        residueThrown = false;
        SpawnResidue();
    } */

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
        CheckGameOver();
        Destroy(currentResidueInstance);
        enabled = false;
    }
}
    public int GetScore()
    {
        return score;
    }

    public float GetTimeRemaining()
    {
        return timeRemaining;
    }


    void CheckGameOver()
    {
        // Verificar si el puntaje es suficiente para ganar
        if (score >= 1050)
        {
            victoryManager.ShowVictoryPanel();
        }
        // Verificar si el puntaje es demasiado bajo para perder
        else if (score <= 1000)
        {
            defeatManager.CheckForTimeOutDefeat();
        }
    }

}



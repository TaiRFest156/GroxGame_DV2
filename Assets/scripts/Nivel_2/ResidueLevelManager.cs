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

    void Start()
    {
        Input.gyro.enabled = true;
        timeRemaining = levelDuration;
        SpawnResidue();
    }

    void Update()
    {
        UpdateTimer();

        if (!residueThrown && currentResidueInstance != null)
            CheckTilt();
    }

    void CheckTilt()
    {
        Vector3 tilt = Input.acceleration;

        // Giroscopio
        bool leftTilt = tilt.x < -0.35f;
        bool rightTilt = tilt.x > 0.35f;
        bool upTilt = tilt.y > 0.45f;

        // Teclado
        bool leftKey = Input.GetKeyDown(KeyCode.A);
        bool rightKey = Input.GetKeyDown(KeyCode.D);
        bool upKey = Input.GetKeyDown(KeyCode.W);

        if (!residueThrown && (leftTilt || leftKey))
            StartCoroutine(ThrowResidue(targetLeft, ResidueKind.Papel));
        else if (!residueThrown && (rightTilt || rightKey))
            StartCoroutine(ThrowResidue(targetRight, ResidueKind.Plastico));
        else if (!residueThrown && (upTilt || upKey))
            StartCoroutine(ThrowResidue(targetUp, ResidueKind.Vidrio));
    }

    IEnumerator ThrowResidue(Transform targetPoint, ResidueKind directionKind)
    {
        residueThrown = true;

        Vector3 startPos = currentResidueInstance.transform.position;
        Vector3 endPos = targetPoint.position;
        endPos.z = startPos.z; // Mantener el Z original para que no se "salga" de cámara

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * launchSpeed;
            float smooth = Mathf.SmoothStep(0f, 1f, t);
            currentResidueInstance.transform.position = Vector3.Lerp(startPos, endPos, smooth);
            yield return null;
        }

        // Asegurar posición final exacta
        currentResidueInstance.transform.position = endPos;

        // Puntaje
        if (currentResidueData.kind == directionKind)
            score += currentResidueData.scoreValue;
        else
            score += currentResidueData.penaltyValue;

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
}

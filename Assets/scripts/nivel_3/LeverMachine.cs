using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LeverMachine : MonoBehaviour, IInteractable
{
    [Header("Shape Keys")]
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private string leverKeyName = "Key 2";   // Palanca
    [SerializeField] private string pressKeyName = "Key 1";   // Prensa
    [SerializeField] private float leverDuration = 0.5f;      // Tiempo que tarda en bajar la palanca
    [SerializeField] private float pressDuration = 1f;        // Prensa: mismo tiempo al bajar y al subir

    [Header("Delay")]
    [SerializeField] private float delayBeforePress = 0.5f;   // Pausa entre palanca y prensa

    [Header("Prensa (Key 1)")]
    [SerializeField] private float pressStartValue = 0f;      // Valor inicial de Key 1
    [SerializeField] private float pressEndValue = 100f;      // Valor final de Key 1
    [SerializeField] private float objectStartThreshold = 38.8f; // Al llegar a este valor, el objeto empieza a bajar

    [Header("Objeto que baja en Y")]
    [SerializeField] private Transform movingObject;
    [SerializeField] private float moveDownDistance = 0.5f;   // Cuánto baja en Y (unidades locales)
    [SerializeField] private bool returnObjectOnReset = true; // Sube junto con la prensa al resetear

    [Header("Reset")]
    [SerializeField] private float leverResetDuration = 0.05f; // Tiempo que tarda en subir la palanca

    [Header("Spawner")]
    [SerializeField] private ObjectSpawner spawner;

    [Header("Eventos")]
    public UnityEvent onMinigameReady;   // Aquí conectarás el minijuego después

    private int leverIndex, pressIndex;
    private bool running;
    private bool objectArrived;
    private Vector3 objectStartPos;

    public bool CanInteract => !running;

    private void Start()
    {
        leverIndex = meshRenderer.sharedMesh.GetBlendShapeIndex(leverKeyName);
        pressIndex = meshRenderer.sharedMesh.GetBlendShapeIndex(pressKeyName);

        if (leverIndex < 0 || pressIndex < 0)
            Debug.LogError("No encontré las shape keys. Revisa los nombres en el Inspector.");

        if (movingObject != null) objectStartPos = movingObject.localPosition;

        spawner.onArrived.AddListener(() => objectArrived = true);
    }

    public void Interact()
    {
        if (!running) StartCoroutine(Sequence());
    }

    private IEnumerator Sequence()
    {
        running = true;
        objectArrived = false;

        // Posición inicial
        meshRenderer.SetBlendShapeWeight(leverIndex, 0);
        meshRenderer.SetBlendShapeWeight(pressIndex, pressStartValue);
        if (movingObject != null) movingObject.localPosition = objectStartPos;

        // 1. Baja la palanca (Key 2: 0 -> 100)
        yield return AnimateLever(0, 100, leverDuration);

        // 2. Delay
        yield return new WaitForSeconds(delayBeforePress);

        // 3. Prensa baja (Key 1: 0 -> 100) + objeto bajando en Y
        yield return AnimatePress(pressStartValue, pressEndValue, pressDuration);

        // 4. Crea 1 objeto
        spawner.Activate();

        // 5. Espera a que llegue al punto B (o a que lo destruyan antes)
        yield return new WaitUntil(() => objectArrived || spawner.Current == null);

        // 6. Minijuego (pendiente)
        if (spawner.Current != null) onMinigameReady?.Invoke();

        // 7. Espera a que el objeto se destruya
        yield return new WaitUntil(() => spawner.Current == null);

        // 8. Sube la palanca (rápido) y la prensa regresa en el mismo tiempo que bajó
        StartCoroutine(AnimateLever(100, 0, leverResetDuration));
        yield return AnimatePress(pressEndValue, pressStartValue, pressDuration);

        running = false;
    }

    private IEnumerator AnimateLever(float from, float to, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            meshRenderer.SetBlendShapeWeight(leverIndex, Mathf.Lerp(from, to, t / duration));
            yield return null;
        }
        meshRenderer.SetBlendShapeWeight(leverIndex, to);
    }

    private IEnumerator AnimatePress(float from, float to, float duration)
    {
        bool goingBack = to == pressStartValue;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float value = Mathf.Lerp(from, to, t / duration);
            meshRenderer.SetBlendShapeWeight(pressIndex, value);
            if (!goingBack || returnObjectOnReset) MoveObject(value);
            yield return null;
        }
        meshRenderer.SetBlendShapeWeight(pressIndex, to);
        if (!goingBack || returnObjectOnReset) MoveObject(to);
    }

    // 0 cuando Key 1 está en el umbral (38.8), 1 cuando llega al valor final
    private void MoveObject(float pressValue)
    {
        if (movingObject == null) return;

        float progress = Mathf.InverseLerp(objectStartThreshold, pressEndValue, pressValue);
        movingObject.localPosition = objectStartPos + Vector3.down * (moveDownDistance * progress);
    }
}
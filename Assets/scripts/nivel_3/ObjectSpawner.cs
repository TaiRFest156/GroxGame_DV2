using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Objeto")]
    [SerializeField] private GameObject prefab;      // Lo que se va a spawnear

    [Header("Puntos")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Movimiento")]
    [SerializeField] private float speed = 3f;       // Unidades por segundo
    [SerializeField] private bool destroyAtEnd = false;

    public UnityEvent onSpawned;
    public UnityEvent onArrived;

    private GameObject current;

    // Objeto actual (null si ya fue destruido)
    public GameObject Current => current;

    // Lo llama otro script
    public void Activate()
    {
        if (current != null) Destroy(current);       // respawn: elimina el anterior
        current = Instantiate(prefab, pointA.position, pointA.rotation);
        onSpawned?.Invoke();
        StartCoroutine(MoveRoutine(current.transform));
    }

    private IEnumerator MoveRoutine(Transform obj)
    {
        while (obj != null && Vector3.Distance(obj.position, pointB.position) > 0.01f)
        {
            obj.position = Vector3.MoveTowards(obj.position, pointB.position, speed * Time.deltaTime);
            yield return null;
        }

        if (obj == null) yield break;
        obj.position = pointB.position;
        onArrived?.Invoke();
        if (destroyAtEnd) Destroy(obj.gameObject);
    }

    // Para cambiar la velocidad desde otros scripts
    public void SetSpeed(float newSpeed) => speed = newSpeed;
}
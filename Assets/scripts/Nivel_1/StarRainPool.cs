using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StarRainPool : MonoBehaviour
{
    public static StarRainPool Instance { get; private set; }

    [Header("Prefab de estrella")]
    public GameObject starPrefab;

    [Header("Puntos de origen")]
    public Transform[] puntosOrigen;

    [Header("Configuración")]
    public int poolSize = 20;
    public float speed = 30f;
    public float destinoZ = -200f;
    public float intervaloSpawn = 0.2f;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private int origenIndex = 0; // índice para rotar los puntos de origen

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(starPrefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnStar();
            yield return new WaitForSeconds(intervaloSpawn);
        }
    }

    void SpawnStar()
    {
        if (pool.Count == 0 || puntosOrigen.Length == 0)
            return;

        GameObject obj = pool.Dequeue();

        // Selección cíclica de origen
        Transform origen = puntosOrigen[origenIndex];
        origenIndex = (origenIndex + 1) % puntosOrigen.Length;

        obj.transform.position = origen.position;
        obj.SetActive(true);

        StartCoroutine(MoverEstrella(obj));
    }

    IEnumerator MoverEstrella(GameObject obj)
    {
        while (obj.transform.position.z > destinoZ)
        {
            obj.transform.position += Vector3.back * speed * Time.deltaTime;
            yield return null;
        }

        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}

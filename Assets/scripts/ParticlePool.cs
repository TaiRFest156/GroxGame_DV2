using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticlePool : MonoBehaviour
{
    public static ParticlePool Instance { get; private set; }

    [Header("Prefabs de partículas")]
    public GameObject particlePrefab1;
    public GameObject particlePrefab2;
    public int poolSize = 10;

    private Queue<GameObject> pool1 = new Queue<GameObject>();
    private Queue<GameObject> pool2 = new Queue<GameObject>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // evita duplicados
    }

    void Start()
    {
        // Inicializa ambos pools
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj1 = Instantiate(particlePrefab1);
            obj1.SetActive(false);
            pool1.Enqueue(obj1);

            GameObject obj2 = Instantiate(particlePrefab2);
            obj2.SetActive(false);
            pool2.Enqueue(obj2);
        }
    }

    public void PlayEffect(Vector3 position)
    {
        PlayFromPool(pool1, position);
        PlayFromPool(pool2, position);
    }

    private void PlayFromPool(Queue<GameObject> pool, Vector3 position)
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.transform.position = position;
            obj.SetActive(true);

            ParticleSystem ps = obj.GetComponentInChildren<ParticleSystem>();
            if (ps != null)
            {
                ps.Play(true);
                StartCoroutine(Recycle(obj, ps.main.duration, pool));
            }
            else
            {
                Debug.LogWarning("El prefab no tiene ParticleSystem asignado.");
                StartCoroutine(Recycle(obj, 1f, pool));
            }
        }
    }

    IEnumerator Recycle(GameObject obj, float delay, Queue<GameObject> pool)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}

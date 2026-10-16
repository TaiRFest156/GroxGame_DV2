using System.Collections;
using UnityEngine;

public class SpawnBasura : MonoBehaviour
{
    [Header("Prefabs de Basura")]
    public GameObject[] basuraPrefabs;

    [Header("Área de Spawn")]
    public float minX = -5f;
    public float maxX = 5f;
    public float minZ = -5f;
    public float maxZ = 5f;
    public float spawnY = 1f;

    [Header("Tiempo entre spawns")]
    public float tiempoInicial = 1.2f;
    public float tiempoMinimo = 0.3f;
    public float reduccionTiempo = 0.01f;

    [Header("Duración del spawner")]
    public float duracionSpawner = 60f;

    private float tiempoActual;
    private float tiempoTranscurrido = 0f;
    private Coroutine rutinaSpawner;

    // Ahora no arranca solo en Start
    void Awake()
    {
        tiempoActual = tiempoInicial;
    }

    // Método público para iniciar el spawner
    public void IniciarSpawner()
    {
        if (rutinaSpawner == null)
        {
            tiempoTranscurrido = 0f;
            tiempoActual = tiempoInicial;
            rutinaSpawner = StartCoroutine(SpawnRoutine());

            // Llamar al contador
            UIContadorTiempo ui = Object.FindFirstObjectByType<UIContadorTiempo>();
            if (ui != null)
            {
                ui.IniciarContador(duracionSpawner);
            }
        }
    }


    // Método público para detener el spawner antes de tiempo
    public void DetenerSpawner()
    {
        if (rutinaSpawner != null)
        {
            StopCoroutine(rutinaSpawner);
            rutinaSpawner = null;
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (tiempoTranscurrido < duracionSpawner)
        {
            SpawnearBasura();

            yield return new WaitForSeconds(tiempoActual);
            tiempoTranscurrido += tiempoActual;

            if (tiempoActual > tiempoMinimo)
            {
                tiempoActual -= reduccionTiempo;
            }
        }

        rutinaSpawner = null; // se libera al terminar

        SceneLoader loader = Object.FindFirstObjectByType<SceneLoader>();
        if (loader != null)
        {
            loader.OnSpawnerFinished();

            if (!ReciclajeContador.SuperoMeta())
            {
                Time.timeScale = 1f;
                PlayerPrefs.SetString("EscenaAnterior", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                UnityEngine.SceneManagement.SceneManager.LoadScene("Derrota");
            }
        }
    }

    void SpawnearBasura()
    {
        if (basuraPrefabs.Length == 0) return;

        GameObject prefab = basuraPrefabs[Random.Range(0, basuraPrefabs.Length)];
        Vector3 spawnPos = new Vector3(Random.Range(minX, maxX), spawnY, Random.Range(minZ, maxZ));
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 center = new Vector3((minX + maxX) / 2f, spawnY, (minZ + maxZ) / 2f);
        Vector3 size = new Vector3(Mathf.Abs(maxX - minX), 0.1f, Mathf.Abs(maxZ - minZ));
        Gizmos.DrawWireCube(center, size);
    }
}

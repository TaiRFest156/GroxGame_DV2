using System.Collections;
using System.Collections.Generic;
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
    public float spawnY = 1f; // Altura fija

    [Header("Tiempo entre spawns")]
    public float tiempoInicial = 1.2f;
    public float tiempoMinimo = 0.3f;
    public float reduccionTiempo = 0.01f;

    [Header("Duración del spawner")]
    public float duracionSpawner = 60f; // 1 minuto

    public float tiempoActual;
    public float tiempoTranscurrido = 0f;

    void Start()
    {
        tiempoActual = tiempoInicial;
        StartCoroutine(SpawnRoutine());
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

        SceneLoader loader = Object.FindFirstObjectByType<SceneLoader>();
        if (loader != null)
        {
            loader.OnSpawnerFinished();

            //  Aquí verifica si recogió menos de 5 basuras
            if (!ReciclajeContador.RecogioMinimoBasuras(5))
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

        Vector3 spawnPos = new Vector3(
            Random.Range(minX, maxX),
            spawnY,
            Random.Range(minZ, maxZ)
        );

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    // Visualización del área de spawn en el editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 center = new Vector3((minX + maxX) / 2f, spawnY, (minZ + maxZ) / 2f);
        Vector3 size = new Vector3(Mathf.Abs(maxX - minX), 0.1f, Mathf.Abs(maxZ - minZ));
        Gizmos.DrawWireCube(center, size);
    }
}

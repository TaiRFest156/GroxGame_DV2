using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBasura : MonoBehaviour
{
    [Header("Prefabs de Basura")]
    public GameObject[] basuraPrefabs;   // Array de objetos a spawnear

    [Header("Área de Spawn")]
    public float minX = -5f;
    public float maxX = 5f;
    public float spawnY = 6f;

    [Header("Tiempo entre spawns")]
    public float tiempoInicial = 1.2f;
    public float tiempoMinimo = 0.3f;
    public float reduccionTiempo = 0.01f; // cuanto se acelera por segundo

    private float tiempoActual;

    void Start()
    {
        tiempoActual = tiempoInicial;
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnearBasura();

            yield return new WaitForSeconds(tiempoActual);

            // Aumentar dificultad gradualmente
            if (tiempoActual > tiempoMinimo)
            {
                tiempoActual -= reduccionTiempo;
            }
        }
    }

    void SpawnearBasura()
    {
        if (basuraPrefabs.Length == 0) return;

        // Elegir prefab aleatorio
        GameObject prefab = basuraPrefabs[Random.Range(0, basuraPrefabs.Length)];

        // Elegir posición aleatoria en el eje X
        Vector3 spawnPos = new Vector3(Random.Range(minX, maxX), spawnY, 0);

        // Crear el objeto
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}

using System.Collections;
using UnityEngine;

public class NaveMovimiento : MonoBehaviour
{
    [Header("Movimiento")]
    public Transform destino;
    public float duracion = 5f;
    public float curva = 2f;

    [Header("Inicio")]
    public float esperaInicial = 3f;

    [Header("Velocidad")]
    [Range(0.1f, 1f)]
    public float velocidadMedia = 0.5f;

    [Header("Compuerta")]
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public string nombreShapeKey = "open";
    public float aperturaMaxima = 100f;
    public float velocidadApertura = 30f;

    [Header("Cinematica")]
    public CinematicaNave cinematica;

    [Header("Spawner")]
    public SpawnBasura spawner;   // referencia al spawner en el inspector

    private Vector3 inicio;
    private float progreso;
    private int indiceShapeKey = -1;
    private float pesoActual;
    private bool movimientoIniciado;
    private bool spawnerIniciado; // evita múltiples llamadas

    void Start()
    {
        inicio = transform.position;
        movimientoIniciado = false;
        spawnerIniciado = false;

        if (skinnedMeshRenderer != null &&
            skinnedMeshRenderer.sharedMesh != null)
        {
            indiceShapeKey =
                skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(nombreShapeKey);

            if (indiceShapeKey == -1)
                Debug.LogError("No se encontró la Shape Key: " + nombreShapeKey);
        }

        StartCoroutine(IniciarSecuencia());
    }

    void Update()
    {
        if (!movimientoIniciado || destino == null || progreso >= 1f)
            return;

        // Desacelera cerca de la mitad del recorrido
        float factor = 1f - (1f - velocidadMedia) *
            Mathf.Exp(-Mathf.Pow((progreso - 0.5f) / 0.15f, 2f));

        progreso += (Time.deltaTime / duracion) * factor;
        progreso = Mathf.Clamp01(progreso);

        // Movimiento con curva lateral
        Vector3 posicion = Vector3.Lerp(inicio, destino.position, progreso);
        posicion.x += Mathf.Sin(progreso * Mathf.PI) * curva;
        transform.position = posicion;

        // Apertura de compuerta
        float apertura = Mathf.InverseLerp(0.35f, 0.5f, progreso);
        apertura = Mathf.Clamp01(apertura) * aperturaMaxima;

        if (progreso > 0.6f)
        {
            apertura = Mathf.Lerp(aperturaMaxima, 0f,
                Mathf.InverseLerp(0.6f, 0.8f, progreso));
        }

        pesoActual = Mathf.MoveTowards(pesoActual, apertura,
            velocidadApertura * Time.deltaTime);

        if (skinnedMeshRenderer != null && indiceShapeKey >= 0)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(indiceShapeKey, pesoActual);
        }

        // Llamada al spawner cuando la compuerta empieza a abrir
        if (pesoActual > 0f && !spawnerIniciado && spawner != null)
        {
            spawner.IniciarSpawner();
            spawnerIniciado = true; // asegura que solo se llame una vez
        }
    }

    IEnumerator IniciarSecuencia()
    {
        yield return new WaitForSeconds(esperaInicial);

        if (cinematica != null)
            cinematica.IniciarCinematica();

        movimientoIniciado = true;
    }
}

using TMPro;
using UnityEngine;

public class UIContadorTiempo : MonoBehaviour
{
    public SpawnBasura spawner;
    public TextMeshProUGUI textoTiempo;

    private float tiempoRestante;

    void Start()
    {
        tiempoRestante = spawner.duracionSpawner;
    }

    void Update()
    {
        if (tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime;
            if (tiempoRestante < 0) tiempoRestante = 0;
        }

        textoTiempo.text = $"Tiempo restante: {tiempoRestante:F0} s";
    }
}

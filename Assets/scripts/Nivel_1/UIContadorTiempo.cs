using TMPro;
using UnityEngine;

public class UIContadorTiempo : MonoBehaviour
{
    public TextMeshProUGUI textoTiempo;

    private float tiempoRestante;
    private bool contando;

    // Este método lo llama el spawner al iniciar
    public void IniciarContador(float duracion)
    {
        tiempoRestante = duracion;
        contando = true;
    }

    void Update()
    {
        if (!contando) return;

        if (tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime;
            if (tiempoRestante < 0) tiempoRestante = 0;
        }

        textoTiempo.text = $"Tiempo restante: {tiempoRestante:F0} s";
    }
}

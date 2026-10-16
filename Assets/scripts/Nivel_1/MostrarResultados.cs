using UnityEngine;
using TMPro; 

public class MostrarResultados : MonoBehaviour
{
    public TextMeshProUGUI textoResultados; // arrastrá tu objeto de texto desde el inspector

    void Start()
    {
        string resumen = "";

        foreach (var par in ReciclajeContador.conteo)
        {
            resumen += $"Recogiste {par.Value} de {par.Key}\n";
        }

        textoResultados.text = resumen;
    }
}

using System.Collections.Generic;
using UnityEngine;

public static class ReciclajeContador
{
    // Conteo por tipo de basura
    public static Dictionary<string, int> conteo = new Dictionary<string, int>
    {
        { "Metal", 0 },
        { "Plastico", 0 },
        { "Vidrio", 0 },
        { "Toxico", 0 },
        { "Papel", 0 }
    };

    // Valor por unidad de cada tipo
    public static Dictionary<string, int> valores = new Dictionary<string, int>
    {
        { "Metal", 4500 },
        { "Plastico", 3000 },
        { "Vidrio", 5900 },
        { "Toxico", 10000 },
        { "Papel", 12500 }
    };

    // Registrar un objeto recogido
    public static void Registrar(string tipo)
    {
        if (!conteo.ContainsKey(tipo)) return;
        conteo[tipo]++;
    }

    // Resetear el conteo
    public static void Resetear()
    {
        var keys = new List<string>(conteo.Keys);
        foreach (var k in keys) conteo[k] = 0;
    }

    // Calcular puntaje total
    public static int CalcularPuntajeTotal()
    {
        int total = 0;
        foreach (var tipo in conteo.Keys)
        {
            int cantidad = conteo[tipo];
            int valor = valores.ContainsKey(tipo) ? valores[tipo] : 0;
            total += cantidad * valor;
        }
        return total;
    }
}

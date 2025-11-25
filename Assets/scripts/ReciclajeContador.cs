using System.Collections.Generic;
using UnityEngine;

public static class ReciclajeContador
{
    public static Dictionary<string, int> conteo = new Dictionary<string, int>
    {
        { "Metal", 0 },
        { "Plastico", 0 },
        { "Vidrio", 0 },
         { "Toxico", 0 },
        { "Papel", 0 }
    };

    public static void Registrar(string tipo)
    {
        if (!conteo.ContainsKey(tipo)) return; // Ignora tags no registrados
        conteo[tipo]++;
    }

    public static void Resetear()
    {
        var keys = new List<string>(conteo.Keys);
        foreach (var k in keys) conteo[k] = 0;
    }
}

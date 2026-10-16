using System;
using System.Collections.Generic;
using UnityEngine;

public static class ReciclajeContador
{
    // Puntaje mínimo para pasar el nivel (lo define ObjetivoUI)
    public static int MetaPuntaje = 10000;

    // Se dispara cada vez que cambia el conteo
    public static event Action OnConteoCambiado;

    // Conteo por tipo de basura
    public static Dictionary<string, int> conteo = new Dictionary<string, int>
    {
        { "Metal", 0 },
        { "Plastico", 0 },
        { "Vidrio", 0 },
        { "Toxico", 0 },
        { "Papel", 0 },
        { "Organico", 0 }
    };

    // Valor por unidad de cada tipo
    public static Dictionary<string, int> valores = new Dictionary<string, int>
    {
        { "Metal", 4500 },
        { "Plastico", 3000 },
        { "Vidrio", 3500 },
        { "Toxico", 2600 },
        { "Organico", 2000 },
        { "Papel", 1500 }
    };

    // Registrar un objeto recogido
    public static void Registrar(string tipo)
    {
        if (!conteo.ContainsKey(tipo)) return;
        conteo[tipo]++;
        OnConteoCambiado?.Invoke();
    }

    // Resetear el conteo
    public static void Resetear()
    {
        var keys = new List<string>(conteo.Keys);
        foreach (var k in keys) conteo[k] = 0;
        OnConteoCambiado?.Invoke();
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

    // ¿Alcanzó la meta de puntaje?
    public static bool SuperoMeta()
    {
        return CalcularPuntajeTotal() >= MetaPuntaje;
    }
}
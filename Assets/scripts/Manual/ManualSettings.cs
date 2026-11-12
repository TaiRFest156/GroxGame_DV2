using System.Collections.Generic;
using UnityEngine;

// Preferencias del manual. Se guardan en el dispositivo (PlayerPrefs),
// asi que sobreviven al cerrar y volver a abrir el juego.
public static class ManualSettings
{
    private const string ClaveAutomatico = "Manual_AbrirAutomatico";

    // Niveles cuyo "¡Aprendamos a jugar!" ya se mostro en esta sesion (se borra al cerrar el juego).
    private static readonly HashSet<string> introsMostradas = new HashSet<string>();

    // true = el manual se abre solo al empezar un nivel (por defecto)
    public static bool AbrirAutomatico
    {
        get { return PlayerPrefs.GetInt(ClaveAutomatico, 1) == 1; }
        set
        {
            PlayerPrefs.SetInt(ClaveAutomatico, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool IntroYaMostrada(string escena)
    {
        return introsMostradas.Contains(escena);
    }

    public static void MarcarIntroMostrada(string escena)
    {
        introsMostradas.Add(escena);
    }
}

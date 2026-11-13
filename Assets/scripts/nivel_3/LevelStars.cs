using UnityEngine;

// Estrellas (0 a 3) guardadas por nivel. Pensado para usarse en los 3 niveles.
// Uso:  LevelStars.Save(3, 2);   int e = LevelStars.Get(3);
public static class LevelStars
{
    private static string Clave(int nivel) { return "Estrellas_Nivel" + nivel; }

    public static int Get(int nivel)
    {
        return PlayerPrefs.GetInt(Clave(nivel), 0);
    }

    // Solo guarda si mejora la marca anterior
    public static void Save(int nivel, int estrellas)
    {
        estrellas = Mathf.Clamp(estrellas, 0, 3);
        if (estrellas > Get(nivel))
        {
            PlayerPrefs.SetInt(Clave(nivel), estrellas);
            PlayerPrefs.Save();
        }
    }
}

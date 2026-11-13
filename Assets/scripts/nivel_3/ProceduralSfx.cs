using UnityEngine;

// Sonidos generados por codigo para que el nivel suene sin necesitar archivos de audio.
// Tu sonidista puede reemplazarlos asignando clips propios en el Inspector.
public static class ProceduralSfx
{
    private const int Rate = 44100;

    // Pentatonica: C4 E4 G4 C5 E5 G5 (una nota por color, siempre suenan bien juntas)
    private static readonly float[] Frecuencias = { 261.63f, 329.63f, 392.00f, 523.25f, 659.25f, 783.99f };

    private static AudioClip[] notas = new AudioClip[6];
    private static AudioClip error, exito, whoosh, ding;

    public static AudioClip Nota(int i)
    {
        i = Mathf.Clamp(i, 0, Frecuencias.Length - 1);
        if (notas[i] == null) notas[i] = Tono("nota" + i, Frecuencias[i], 0.5f, 0.35f);
        return notas[i];
    }

    public static AudioClip Ding()
    {
        if (ding == null) ding = Tono("ding", 1046.5f, 0.45f, 0.3f);
        return ding;
    }

    public static AudioClip Error()
    {
        if (error != null) return error;
        int n = (int)(Rate * 0.45f);
        float[] d = new float[n];
        float fase = 0f;
        for (int i = 0; i < n; i++)
        {
            float t = i / (float)n;
            float f = Mathf.Lerp(190f, 80f, t);
            fase += 2f * Mathf.PI * f / Rate;
            float cuadrada = Mathf.Sign(Mathf.Sin(fase)) * 0.5f + Mathf.Sin(fase) * 0.5f;
            d[i] = cuadrada * 0.35f * (1f - t);
        }
        error = Crear("error", d);
        return error;
    }

    public static AudioClip Exito()
    {
        if (exito != null) return exito;
        float[] notasHz = { 523.25f, 659.25f, 783.99f, 1046.5f };
        int porNota = (int)(Rate * 0.13f);
        int n = porNota * notasHz.Length + (int)(Rate * 0.35f);
        float[] d = new float[n];
        for (int k = 0; k < notasHz.Length; k++)
        {
            int inicio = k * porNota;
            int largo = (k == notasHz.Length - 1) ? n - inicio : porNota * 3;
            for (int i = 0; i < largo && inicio + i < n; i++)
            {
                float t = i / (float)Rate;
                float env = Mathf.Min(1f, i / (Rate * 0.01f)) * Mathf.Exp(-4f * t);
                d[inicio + i] += 0.28f * env * Mathf.Sin(2f * Mathf.PI * notasHz[k] * t);
            }
        }
        exito = Crear("exito", d);
        return exito;
    }

    public static AudioClip Whoosh()
    {
        if (whoosh != null) return whoosh;
        int n = (int)(Rate * 0.6f);
        float[] d = new float[n];
        float fase = 0f;
        System.Random azar = new System.Random(7);
        for (int i = 0; i < n; i++)
        {
            float t = i / (float)n;
            float f = Mathf.Lerp(180f, 1500f, t * t);
            fase += 2f * Mathf.PI * f / Rate;
            float ruido = (float)(azar.NextDouble() * 2.0 - 1.0);
            float env = Mathf.Sin(t * Mathf.PI);
            d[i] = (Mathf.Sin(fase) * 0.5f + ruido * 0.25f) * env * 0.35f;
        }
        whoosh = Crear("whoosh", d);
        return whoosh;
    }

    private static AudioClip Tono(string nombre, float freq, float dur, float vol)
    {
        int n = (int)(Rate * dur);
        float[] d = new float[n];
        for (int i = 0; i < n; i++)
        {
            float t = i / (float)Rate;
            float env = Mathf.Min(1f, i / (Rate * 0.01f)) * Mathf.Exp(-3.5f * t / dur);
            float onda = Mathf.Sin(2f * Mathf.PI * freq * t) + 0.3f * Mathf.Sin(4f * Mathf.PI * freq * t);
            d[i] = vol * env * onda / 1.3f;
        }
        return Crear(nombre, d);
    }

    private static AudioClip Crear(string nombre, float[] datos)
    {
        AudioClip c = AudioClip.Create(nombre, datos.Length, 1, Rate, false);
        c.SetData(datos, 0);
        return c;
    }
}

using UnityEngine;

// Genera por codigo los sprites simples del manual (rectangulo redondeado, circulo, triangulo)
// para que el manual se vea bien sin necesitar imagenes en el proyecto.
public static class ManualSpriteFactory
{
    private static Sprite rectRedondeado;
    private static Sprite circulo;
    private static Sprite triangulo;

    public static Sprite RectRedondeado()
    {
        if (rectRedondeado != null) return rectRedondeado;

        const int n = 64;
        const float radio = 20f;
        float mitad = n * 0.5f;
        var colores = new Color32[n * n];

        for (int y = 0; y < n; y++)
        {
            for (int x = 0; x < n; x++)
            {
                float qx = Mathf.Abs(x + 0.5f - mitad) - (mitad - radio);
                float qy = Mathf.Abs(y + 0.5f - mitad) - (mitad - radio);
                float ox = Mathf.Max(qx, 0f);
                float oy = Mathf.Max(qy, 0f);
                float d = Mathf.Sqrt(ox * ox + oy * oy) + Mathf.Min(Mathf.Max(qx, qy), 0f) - radio;
                float a = Mathf.Clamp01(0.5f - d);
                colores[y * n + x] = new Color32(255, 255, 255, (byte)(a * 255f));
            }
        }

        rectRedondeado = Crear(colores, n, new Vector4(radio, radio, radio, radio));
        return rectRedondeado;
    }

    public static Sprite Circulo()
    {
        if (circulo != null) return circulo;

        const int n = 128;
        float mitad = n * 0.5f;
        float radio = mitad - 1f;
        var colores = new Color32[n * n];

        for (int y = 0; y < n; y++)
        {
            for (int x = 0; x < n; x++)
            {
                float dx = x + 0.5f - mitad;
                float dy = y + 0.5f - mitad;
                float d = Mathf.Sqrt(dx * dx + dy * dy) - radio;
                float a = Mathf.Clamp01(0.5f - d);
                colores[y * n + x] = new Color32(255, 255, 255, (byte)(a * 255f));
            }
        }

        circulo = Crear(colores, n, Vector4.zero);
        return circulo;
    }

    // Triangulo apuntando a la derecha (para la izquierda se voltea con scale x = -1)
    public static Sprite Triangulo()
    {
        if (triangulo != null) return triangulo;

        const int n = 128;
        var colores = new Color32[n * n];

        Vector2 a = new Vector2(0.25f, 0.15f);
        Vector2 b = new Vector2(0.80f, 0.50f);
        Vector2 c = new Vector2(0.25f, 0.85f);

        for (int y = 0; y < n; y++)
        {
            for (int x = 0; x < n; x++)
            {
                Vector2 p = new Vector2((x + 0.5f) / n, (y + 0.5f) / n);
                float d = Mathf.Min(DistanciaInterior(p, a, b),
                          Mathf.Min(DistanciaInterior(p, b, c), DistanciaInterior(p, c, a)));
                float alfa = Mathf.Clamp01(d * n + 0.5f);
                colores[y * n + x] = new Color32(255, 255, 255, (byte)(alfa * 255f));
            }
        }

        triangulo = Crear(colores, n, Vector4.zero);
        return triangulo;
    }

    // Distancia con signo (positiva dentro) a la arista p0->p1 de un triangulo antihorario
    private static float DistanciaInterior(Vector2 p, Vector2 p0, Vector2 p1)
    {
        Vector2 e = p1 - p0;
        Vector2 normal = new Vector2(-e.y, e.x).normalized;
        return Vector2.Dot(p - p0, normal);
    }

    private static Sprite Crear(Color32[] colores, int n, Vector4 borde)
    {
        var tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;
        tex.SetPixels32(colores);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, n, n), new Vector2(0.5f, 0.5f),
                             100f, 0, SpriteMeshType.FullRect, borde);
    }
}

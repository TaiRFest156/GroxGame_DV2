using UnityEngine;

// Utilidades compartidas por el Nivel 3.

// Enciende/apaga un objeto de color. Funciona en Built-in y URP, con o sin emision.
public static class GlowUtil
{
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorProp = Shader.PropertyToID("_Color");
    private static readonly int Emission = Shader.PropertyToID("_EmissionColor");
    private static readonly Color Apagado = new Color(0.12f, 0.12f, 0.14f);

    // k: 0 = apagado, 0.4 = reposo, 1 = encendido, hasta 2 = brillo fuerte
    public static void Set(Renderer r, Color c, float k)
    {
        if (r == null) return;
        Material m = r.material;   // instancia propia (no toca el material compartido)

        // Aunque el material no tenga emision, el color cambia y se nota el encendido
        Color baseC = Color.Lerp(Apagado, c, Mathf.Clamp01(0.2f + 0.8f * Mathf.Min(k, 1f)));
        if (m.HasProperty(BaseColor)) m.SetColor(BaseColor, baseC);
        if (m.HasProperty(ColorProp)) m.SetColor(ColorProp, baseC);

        if (m.HasProperty(Emission))
        {
            m.EnableKeyword("_EMISSION");
            float i = k * 2.2f;
            m.SetColor(Emission, new Color(c.r * i, c.g * i, c.b * i, 1f));
        }
    }
}

// Acceso al jugador sin depender de como este armado.
public static class JugadorN3
{
    private static Camera cam;

    public static Camera Camara
    {
        get { if (cam == null) cam = Camera.main; return cam; }
    }

    // La camara son los "ojos": su posicion XZ es donde esta parado el jugador
    public static Vector3 PosicionOjos
    {
        get { Camera c = Camara; return c != null ? c.transform.position : Vector3.zero; }
    }

    public static Transform Raiz
    {
        get
        {
            CharacterController cc = Object.FindFirstObjectByType<CharacterController>();
            if (cc != null) return cc.transform;
            Camera c = Camara;
            return c != null ? c.transform.root : null;
        }
    }

    // Mueve al jugador (conservando su altura) y lo gira hacia 'yaw'
    public static void Mover(Transform raiz, Vector3 pos, float yaw)
    {
        if (raiz == null) return;
        CharacterController cc = raiz.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;   // si no, el CharacterController pelea contra el cambio
        raiz.position = new Vector3(pos.x, raiz.position.y, pos.z);
        raiz.rotation = Quaternion.Euler(0f, yaw, 0f);
        if (cc != null) cc.enabled = true;
    }
}

// Chispas de particulas generadas por codigo (no necesita prefabs).
public static class Efectos
{
    private static Material matChispa;

    public static void Chispas(Vector3 pos, Color color, int cantidad = 28)
    {
        GameObject go = new GameObject("Chispas");
        go.transform.position = pos;

        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.loop = false;
        main.duration = 0.4f;
        main.startLifetime = 0.7f;
        main.startSpeed = 3.5f;
        main.startSize = 0.09f;
        main.startColor = color;
        main.gravityModifier = 0.6f;

        var em = ps.emission;
        em.rateOverTime = 0f;
        em.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, (short)cantidad) });

        var sh = ps.shape;
        sh.shapeType = ParticleSystemShapeType.Sphere;
        sh.radius = 0.1f;

        ParticleSystemRenderer rend = go.GetComponent<ParticleSystemRenderer>();
        if (matChispa == null)
        {
            Shader s = Shader.Find("Sprites/Default");
            if (s != null) matChispa = new Material(s);
        }
        if (matChispa != null) rend.sharedMaterial = matChispa;

        ps.Play();
        Object.Destroy(go, 2f);
    }
}

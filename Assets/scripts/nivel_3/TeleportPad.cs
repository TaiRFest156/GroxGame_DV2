using System;
using UnityEngine;

// Teletransportador del Nivel 3. No necesita collider: detecta si el jugador esta parado encima.
// Agregalo al objeto visual del teleport (menu Grox > Nivel 3 > 2).
public class TeleportPad : MonoBehaviour
{
    public Color colorActivo = new Color(0.25f, 0.95f, 1f);
    [Tooltip("0 = usa el tamano del objeto. Si el teleport tiene forma rara, pon un radio en metros.")]
    public float radioActivacion = 0f;

    public event Action AlPisar;

    private Renderer[] rends;
    private Light luz;
    private bool activo;

    private void Awake()
    {
        rends = GetComponentsInChildren<Renderer>();

        GameObject go = new GameObject("LuzTeleport");
        go.transform.SetParent(transform, false);
        go.transform.localPosition = Vector3.up * 0.8f;
        luz = go.AddComponent<Light>();
        luz.type = LightType.Point;
        luz.color = colorActivo;
        luz.range = 4f;
        luz.intensity = 0f;
    }

    public void Activar(bool encendido)
    {
        activo = encendido;
        if (!encendido && luz != null) luz.intensity = 0f;
    }

    private void Update()
    {
        if (!activo || Time.timeScale == 0f) return;

        luz.intensity = 2.5f + 2.5f * (0.5f + 0.5f * Mathf.Sin(Time.time * 5f));

        if (JugadorEncima())
        {
            activo = false;
            luz.intensity = 0f;
            if (AlPisar != null) AlPisar();
        }
    }

    private bool JugadorEncima()
    {
        Vector3 p = JugadorN3.PosicionOjos;

        if (radioActivacion > 0f || rends == null || rends.Length == 0)
        {
            float r = radioActivacion > 0f ? radioActivacion : 0.8f;
            Vector2 d = new Vector2(p.x - transform.position.x, p.z - transform.position.z);
            return d.magnitude <= r;
        }

        Bounds b = rends[0].bounds;
        for (int i = 1; i < rends.Length; i++) b.Encapsulate(rends[i].bounds);
        return b.Contains(new Vector3(p.x, b.center.y, p.z));
    }
}

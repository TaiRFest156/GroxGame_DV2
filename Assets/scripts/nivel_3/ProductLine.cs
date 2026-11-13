using System;
using System.Collections;
using UnityEngine;

public enum TipoProducto { Botella = 0, Bolsa = 1, Remera = 2 }

// Segunda cinta del Nivel 3: aqui sale el producto fabricado y salta a la estanteria.
// Si no asignas prefabs, crea productos simples con formas basicas.
public class ProductLine : MonoBehaviour
{
    [Header("Productos (opcional: tus propios modelos, en orden Botella, Bolsa, Remera)")]
    public GameObject[] prefabs = new GameObject[3];

    [Header("Cinta 2")]
    public Transform inicio;
    public Transform fin;
    public float velocidadCinta = 1.5f;

    [Header("Estanteria (un lugar por producto, en el mismo orden)")]
    public Transform[] slotsEstanteria = new Transform[3];

    [Header("Placeholder")]
    public float escalaProducto = 1f;

    public IEnumerator Fabricar(TipoProducto tipo, Action alAterrizar)
    {
        GameObject prod = CrearProducto(tipo);
        Transform t = prod.transform;
        Vector3 escalaFinal = t.localScale;
        t.position = inicio != null ? inicio.position : transform.position;
        t.localScale = Vector3.zero;

        // Aparece con un pequeno rebote
        float k = 0f;
        while (k < 1f)
        {
            k += Time.deltaTime / 0.3f;
            t.localScale = escalaFinal * EaseOutBack(Mathf.Clamp01(k));
            yield return null;
        }
        t.localScale = escalaFinal;

        // Viaja por la cinta
        if (fin != null)
        {
            while ((t.position - fin.position).sqrMagnitude > 0.0004f)
            {
                t.position = Vector3.MoveTowards(t.position, fin.position, velocidadCinta * Time.deltaTime);
                yield return null;
            }
        }

        // Salta a su lugar en la estanteria
        int idx = (int)tipo;
        if (slotsEstanteria != null && idx < slotsEstanteria.Length && slotsEstanteria[idx] != null)
        {
            Vector3 a = t.position;
            Vector3 b = slotsEstanteria[idx].position;
            Quaternion giroInicial = t.rotation;
            k = 0f;
            while (k < 1f)
            {
                k += Time.deltaTime / 0.7f;
                float e = Mathf.Clamp01(k);
                Vector3 pos = Vector3.Lerp(a, b, e);
                pos.y += Mathf.Sin(e * Mathf.PI) * 0.8f;
                t.position = pos;
                t.rotation = Quaternion.AngleAxis(360f * e, Vector3.up) * giroInicial;
                yield return null;
            }
            t.position = b;
            t.rotation = giroInicial;

            // Luz verde permanente: el cuarto se va llenando de vida
            GameObject lg = new GameObject("LuzProducto");
            lg.transform.position = b + Vector3.up * 0.6f;
            Light l = lg.AddComponent<Light>();
            l.type = LightType.Point;
            l.color = new Color(0.5f, 1f, 0.7f);
            l.range = 2.5f;
            l.intensity = 1.4f;
            Efectos.Chispas(b + Vector3.up * 0.2f, new Color(0.5f, 1f, 0.7f), 20);
        }

        if (alAterrizar != null) alAterrizar();
    }

    private static float EaseOutBack(float x)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }

    // ------------------------------------------------------------------ productos de relleno
    private GameObject CrearProducto(TipoProducto tipo)
    {
        int idx = (int)tipo;
        if (prefabs != null && idx < prefabs.Length && prefabs[idx] != null)
            return Instantiate(prefabs[idx]);

        GameObject raiz = new GameObject("Producto_" + tipo);
        Transform r = raiz.transform;

        switch (tipo)
        {
            case TipoProducto.Botella:
            {
                Color c = new Color(0.3f, 0.8f, 0.95f);
                Parte(r, PrimitiveType.Cylinder, new Vector3(0f, 0.14f, 0f), new Vector3(0.14f, 0.14f, 0.14f), c, Vector3.zero);
                Parte(r, PrimitiveType.Cylinder, new Vector3(0f, 0.33f, 0f), new Vector3(0.06f, 0.05f, 0.06f), c, Vector3.zero);
                Parte(r, PrimitiveType.Cylinder, new Vector3(0f, 0.405f, 0f), new Vector3(0.07f, 0.015f, 0.07f), Color.white, Vector3.zero);
                break;
            }
            case TipoProducto.Bolsa:
            {
                Color c = new Color(0.45f, 0.85f, 0.45f);
                Parte(r, PrimitiveType.Cube, new Vector3(0f, 0.17f, 0f), new Vector3(0.3f, 0.34f, 0.06f), c, Vector3.zero);
                Parte(r, PrimitiveType.Cube, new Vector3(-0.08f, 0.38f, 0f), new Vector3(0.025f, 0.1f, 0.03f), c, Vector3.zero);
                Parte(r, PrimitiveType.Cube, new Vector3(0.08f, 0.38f, 0f), new Vector3(0.025f, 0.1f, 0.03f), c, Vector3.zero);
                Parte(r, PrimitiveType.Cube, new Vector3(0f, 0.43f, 0f), new Vector3(0.185f, 0.025f, 0.03f), c, Vector3.zero);
                break;
            }
            default:
            {
                Color c = new Color(1f, 0.62f, 0.3f);
                Parte(r, PrimitiveType.Cube, new Vector3(0f, 0.17f, 0f), new Vector3(0.26f, 0.34f, 0.05f), c, Vector3.zero);
                Parte(r, PrimitiveType.Cube, new Vector3(-0.19f, 0.28f, 0f), new Vector3(0.16f, 0.1f, 0.05f), c, new Vector3(0f, 0f, 25f));
                Parte(r, PrimitiveType.Cube, new Vector3(0.19f, 0.28f, 0f), new Vector3(0.16f, 0.1f, 0.05f), c, new Vector3(0f, 0f, -25f));
                Parte(r, PrimitiveType.Cube, new Vector3(0f, 0.345f, 0f), new Vector3(0.1f, 0.03f, 0.055f), c * 0.7f, Vector3.zero);
                break;
            }
        }

        r.localScale = Vector3.one * escalaProducto;
        return raiz;
    }

    private static void Parte(Transform padre, PrimitiveType tipo, Vector3 pos, Vector3 escala, Color color, Vector3 giro)
    {
        GameObject g = GameObject.CreatePrimitive(tipo);
        Collider col = g.GetComponent<Collider>();
        if (col != null) Destroy(col);   // que no estorbe al jugador
        g.transform.SetParent(padre, false);
        g.transform.localPosition = pos;
        g.transform.localScale = escala;
        g.transform.localRotation = Quaternion.Euler(giro);
        g.GetComponent<Renderer>().material.color = color;
    }
}

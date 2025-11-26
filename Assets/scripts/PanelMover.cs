using UnityEngine;
using System;

public class PanelMover : MonoBehaviour
{
    public RectTransform panel;             // Asigna tu panel en el Inspector
    public Vector2 destino = Vector2.zero;  // Posición final (centro)
    public float velocidad = 500f;          // Velocidad de movimiento en unidades por segundo

    private bool moviendo = false;

    // Evento que se dispara al terminar el movimiento
    public Action OnMovimientoTerminado;

    void Start()
    {
        if (panel != null)
            panel.anchoredPosition = new Vector2(0, -1038);
    }

    void Update()
    {
        if (!moviendo) return;

        panel.anchoredPosition = Vector2.MoveTowards(
            panel.anchoredPosition,
            destino,
            velocidad * Time.unscaledDeltaTime
        );

        if (Vector2.Distance(panel.anchoredPosition, destino) < 0.1f)
        {
            panel.anchoredPosition = destino;
            moviendo = false;

            // Avisar que terminó
            OnMovimientoTerminado?.Invoke();
        }
    }

    public void ActivarMovimiento()
    {
        if (panel != null)
        {
            panel.anchoredPosition = new Vector2(0, -1038);
            moviendo = true;
        }
    }
}

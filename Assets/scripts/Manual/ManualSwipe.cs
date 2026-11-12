using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Detecta el deslizamiento horizontal del dedo sobre el libro (lo agrega ManualSystem solo).
public class ManualSwipe : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Action alDeslizarIzquierda;   // dedo hacia la izquierda = pagina siguiente
    public Action alDeslizarDerecha;     // dedo hacia la derecha  = pagina anterior
    public float umbralPantalla = 0.06f; // fraccion del ancho de pantalla

    public void OnBeginDrag(PointerEventData e) { }
    public void OnDrag(PointerEventData e) { }

    public void OnEndDrag(PointerEventData e)
    {
        float dx = e.position.x - e.pressPosition.x;
        float minimo = Screen.width * umbralPantalla;

        if (dx <= -minimo)
        {
            if (alDeslizarIzquierda != null) alDeslizarIzquierda();
        }
        else if (dx >= minimo)
        {
            if (alDeslizarDerecha != null) alDeslizarDerecha();
        }
    }
}

using UnityEngine;

public class SubirCreditos : MonoBehaviour
{
    private Vector2 posInicial = new Vector2(340f, -800f);
    private Vector2 posFinal = new Vector2(340f, 5000f);

    private float velocidad = 100f;

    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        //asignamos la pops inicial
        rectTransform.anchoredPosition = posInicial;
    }

    private void Update()
    {
        //evita que se ejecute el movimiento si ya termino
        if (rectTransform.anchoredPosition == posFinal)
            return;

        //lo mueve hacia la pos final 
        rectTransform.anchoredPosition = Vector2.MoveTowards(
            rectTransform.anchoredPosition,
            posFinal,
            velocidad * Time.deltaTime
        );
    }
}

using System.Collections.Generic;
using UnityEngine;

// Una pagina del manual. Para agregar contenido solo hay que sumar elementos a la lista
// "paginas" del asset ManualGrox (no hace falta tocar ninguna escena ni script).
[System.Serializable]
public class ManualPage
{
    [Tooltip("Texto pequeno sobre el titulo. Ej: CONTROLES, NIVEL 1, RECICLAJE")]
    public string etiqueta;

    public string titulo;

    [TextArea(5, 14)]
    [Tooltip("Admite etiquetas de TextMeshPro: <b>negrita</b>, <i>cursiva</i>, <color=#FF0000>color</color>")]
    public string texto;

    [Tooltip("Opcional. Si la pones, aparece arriba del texto (icono, captura, dibujo).")]
    public Sprite imagen;
}

[CreateAssetMenu(fileName = "ManualGrox", menuName = "Grox/Manual (datos)")]
public class ManualData : ScriptableObject
{
    [Tooltip("Las paginas se muestran de a dos (izquierda y derecha) como un libro.")]
    public List<ManualPage> paginas = new List<ManualPage>();
}

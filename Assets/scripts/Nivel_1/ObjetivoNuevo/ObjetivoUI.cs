using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjetivoUI : MonoBehaviour
{
    [Header("UI")]
    public Slider barra;
    public Image relleno;                  // el "Fill" del slider
    public TextMeshProUGUI textoPuntaje;   // ej: "4500 / 10000"
    public GameObject avisoCompletado;     // opcional

    [Header("Metas de puntaje por dificultad")]
    public int[] metasPorNivel = { 10000, 200000, 500000 }; // botón Nivel 1, 2, 3
    public int metaManual = 0;             // si es > 0, ignora la lista (útil para probar)

    [Header("Otros")]
    public bool resetearAlIniciar = true;
    public Color colorPendiente = new Color(1f, 0.75f, 0.1f);
    public Color colorCompletado = Color.green;

    private int meta;

    void Awake()
    {
        if (metaManual > 0)
            meta = metaManual;
        else
        {
            int idx = Mathf.Clamp(NivelSeleccionado.Nivel - 1, 0, metasPorNivel.Length - 1);
            meta = metasPorNivel[idx];
        }

        ReciclajeContador.MetaPuntaje = meta;
    }

    void OnEnable() { ReciclajeContador.OnConteoCambiado += Actualizar; }
    void OnDisable() { ReciclajeContador.OnConteoCambiado -= Actualizar; }

    void Start()
    {
        if (barra != null)
        {
            barra.minValue = 0f;
            barra.maxValue = 1f;
            barra.wholeNumbers = false;
        }

        if (resetearAlIniciar)
            ReciclajeContador.Resetear(); // ya llama a Actualizar()

        Actualizar();
    }

    void Actualizar()
    {
        int puntaje = ReciclajeContador.CalcularPuntajeTotal();
        bool completado = puntaje >= meta;

        if (barra != null)
            barra.value = Mathf.Clamp01((float)puntaje / meta);

        if (relleno != null)
            relleno.color = completado ? colorCompletado : colorPendiente;

        if (textoPuntaje != null)
        {
            textoPuntaje.text = $"{puntaje} / {meta}";
            textoPuntaje.color = completado ? colorCompletado : Color.white;
        }

        if (avisoCompletado != null)
            avisoCompletado.SetActive(completado);
    }
}
using UnityEngine;

/// <summary>
/// Controla la pistola: dispara la animación (Animator), anima los shape keys (blend shapes),
/// y opcionalmente reproduce sonido y fogonazo.
/// Se llama con Disparar() desde RaycastClickDestroy.
/// </summary>
public class Pistola : MonoBehaviour
{
    [System.Serializable]
    public class ShapeKey
    {
        public string nombre;                              // nombre exacto del shape key
        [Range(0f, 100f)] public float pesoReposo = 0f;    // valor inicial y al que vuelve
        [Range(0f, 100f)] public float pesoMaximo = 100f;  // valor en el punto más fuerte del disparo
        [HideInInspector] public int indice = -1;
    }

    [Header("Animación")]
    public Animator animator;
    public string triggerDisparo = "Disparar";             // parámetro Trigger del Animator

    [Header("Shape Keys")]
    public SkinnedMeshRenderer malla;
    public ShapeKey[] shapeKeys;

    [Tooltip("Nombre del clip de disparo. Si se llena, la duración del shape key se iguala a la del clip.")]
    public string nombreClip = "";
    public float duracionShape = 0.25f;                    // se usa si no se indica nombreClip
    // Reposo -> pico -> reposo (debe empezar y terminar en 0)
    public AnimationCurve curva = new AnimationCurve(
        new Keyframe(0f, 0f), new Keyframe(0.2f, 1f), new Keyframe(1f, 0f));

    [Header("Disparo")]
    public float cadencia = 0.25f;                         // segundos mínimos entre disparos
    public AudioSource audioDisparo;                       // opcional
    public AudioClip sonidoDisparo;                        // opcional
    public ParticleSystem fogonazo;                        // opcional

    private int triggerHash;
    private float ultimoDisparo = -999f;
    private float tiempoShape = -1f;                       // -1 = sin animar

    void Start()
    {
        triggerHash = Animator.StringToHash(triggerDisparo);

        ResolverDuracionDelClip();

        if (malla != null && malla.sharedMesh != null)
        {
            foreach (var sk in shapeKeys)
            {
                sk.indice = malla.sharedMesh.GetBlendShapeIndex(sk.nombre);
                if (sk.indice < 0)
                    Debug.LogWarning($"Pistola: no existe el shape key '{sk.nombre}' en la malla.", this);
            }
        }

        AplicarReposo(); // deja los shape keys en su estado inicial al empezar
    }

    void ResolverDuracionDelClip()
    {
        if (animator == null || string.IsNullOrEmpty(nombreClip)) return;
        if (animator.runtimeAnimatorController == null) return;

        var clips = animator.runtimeAnimatorController.animationClips;
        foreach (var clip in clips)
        {
            if (clip.name == nombreClip)
            {
                duracionShape = clip.length / Mathf.Max(0.01f, animator.speed);
                return;
            }
        }

        string disponibles = "";
        foreach (var clip in clips) disponibles += $"'{clip.name}' ";
        Debug.LogWarning($"Pistola: no encontré el clip '{nombreClip}'. Clips disponibles: {disponibles}", this);
    }

    /// <summary>Devuelve false si todavía está en enfriamiento (cadencia).</summary>
    public bool Disparar()
    {
        if (Time.time < ultimoDisparo + cadencia) return false;
        ultimoDisparo = Time.time;

        if (animator != null)
        {
            animator.ResetTrigger(triggerHash); // permite reiniciar si se dispara rápido
            animator.SetTrigger(triggerHash);
        }

        tiempoShape = 0f;

        if (audioDisparo != null && sonidoDisparo != null)
            audioDisparo.PlayOneShot(sonidoDisparo);

        if (fogonazo != null)
            fogonazo.Play(true);

        return true;
    }

    // LateUpdate para aplicar los pesos después del Animator
    void LateUpdate()
    {
        if (malla == null || tiempoShape < 0f) return;

        tiempoShape += Time.deltaTime;
        float t = Mathf.Clamp01(tiempoShape / Mathf.Max(0.01f, duracionShape));
        float factor = Mathf.Clamp01(curva.Evaluate(t)); // evita pasarse por curvas suavizadas

        foreach (var sk in shapeKeys)
        {
            if (sk.indice >= 0)
                malla.SetBlendShapeWeight(sk.indice, Mathf.Lerp(sk.pesoReposo, sk.pesoMaximo, factor));
        }

        if (t >= 1f)
        {
            tiempoShape = -1f;
            AplicarReposo(); // termina exactamente en el estado inicial
        }
    }

    void AplicarReposo()
    {
        if (malla == null) return;

        foreach (var sk in shapeKeys)
            if (sk.indice >= 0) malla.SetBlendShapeWeight(sk.indice, sk.pesoReposo);
    }
}
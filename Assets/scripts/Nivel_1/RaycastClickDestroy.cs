using UnityEngine;
using UnityEngine.InputSystem;

// Versión compatible con tu ReciclajeContador original (Registrar devuelve void)
// y sin depender de BasuraPoolItem.
public class RaycastClickDestroy : MonoBehaviour
{
    public float rayDistance = 50f;
    public Pistola pistola; // arrastra aquí tu pistola

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Pointer pointer = Pointer.current;
        if (pointer == null || !pointer.press.wasPressedThisFrame) return;

        // Solo la mitad derecha de la pantalla (la izquierda es el joystick)
        if (pointer.position.ReadValue().x <= Screen.width / 2f) return;

        // Dispara animación/shape keys siempre, acierte o no. Si está en enfriamiento, no hace nada.
        if (pistola != null && !pistola.Disparar()) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

#if UNITY_EDITOR
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green, 0.5f);
#endif

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
            ProcesarHit(hit);
    }

    void ProcesarHit(RaycastHit hit)
    {
        string tipo = hit.collider.tag;

        if (tipo == "Metal" || tipo == "Plastico" || tipo == "Vidrio" ||
            tipo == "Papel" || tipo == "Toxico" || tipo == "Organico")
        {
            ReciclajeContador.Registrar(tipo);
            Destroy(hit.collider.gameObject);

            if (ParticlePool.Instance != null)
                ParticlePool.Instance.PlayEffect(hit.point);
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastClickDestroy : MonoBehaviour
{
    public float rayDistance = 50f;

    void Update()
    {
        // 1. RAYCAST SIEMPRE DESDE EL CENTRO
        Ray ray = new Ray(Camera.main.transform.position,
                          Camera.main.transform.forward);

        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green);

        RaycastHit hit;

        // 2. TOQUE EN PANTALLA (MOBILE)
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                ProcesarHit(hit);
            }
        }

        // 3. CLICK MOUSE (EDITOR)
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                ProcesarHit(hit);
            }
        }
    }

    // Método auxiliar para no repetir lógica
    void ProcesarHit(RaycastHit hit)
    {
        string tipo = hit.collider.tag;

        if (tipo == "Metal" || tipo == "Plastico" || tipo == "Vidrio" || tipo == "Papel" || tipo == "Toxico")
        {
            // 1. Lógica del juego (Contador, Destruir)
            ReciclajeContador.Registrar(tipo);
            Destroy(hit.collider.gameObject);
            ParticlePool.Instance.PlayEffect(hit.point);

            // 2. Lógica del Audio (Suena al ACERTAR)
            if (AudioManager.Instance != null)
            {
                // Usamos el sonido de disparo (shootSound)
                AudioManager.Instance.PlaySFX(AudioManager.Instance.shootSound);
            }
        }
    }
}

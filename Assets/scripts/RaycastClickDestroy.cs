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
            //  Verificar que el toque esté en la parte derecha de la pantalla
            Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
            if (touchPos.x > Screen.width / 2) // mitad derecha
            {
                if (Physics.Raycast(ray, out hit, rayDistance))
                {
                    ProcesarHit(hit);
                }
            }
        }

        // 3. CLICK MOUSE (EDITOR)
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            //  También podés simular restricción con el mouse
            Vector2 mousePos = Mouse.current.position.ReadValue();
            if (mousePos.x > Screen.width / 2) // mitad derecha
            {
                if (Physics.Raycast(ray, out hit, rayDistance))
                {
                    ProcesarHit(hit);
                }
            }
        }
    }

    // Método auxiliar para no repetir lógica
    void ProcesarHit(RaycastHit hit)
    {
        string tipo = hit.collider.tag;

        if (tipo == "Metal" || tipo == "Plastico" || tipo == "Vidrio" ||
            tipo == "Papel" || tipo == "Toxico" || tipo == "Organico")
        {
            ReciclajeContador.Registrar(tipo);
            Destroy(hit.collider.gameObject);
            ParticlePool.Instance.PlayEffect(hit.point);
        }
    }
}

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
                if (hit.collider.CompareTag("Trash"))
                {
                    Destroy(hit.collider.gameObject);
                }
            }
        }

        
        // 3. CLICK MOUSE (EDITOR)
        
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                if (hit.collider.CompareTag("Trash"))
                {
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }
}

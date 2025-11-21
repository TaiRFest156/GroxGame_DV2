using UnityEngine;
using UnityEngine.EventSystems;

public class MoveLook : MonoBehaviour
{
    [Header("Sensibilidad del movimiento")]
    public float sensibilidad = 0.15f;

    [Header("Límites para la rotación vertical")]
    public float minY = -80f;
    public float maxY = 80f;

    private Vector2 lookInput;
    private float rotX;
    private float rotY;

    private int fingerID = -1; // ID del dedo que está moviendo la cámara

    void Start()
    {
        // Evita problemas en PC: solo tocar en móvil
#if UNITY_EDITOR
        Cursor.lockState = CursorLockMode.None;
#endif
    }

    void Update()
    {
        DetectarMovimientoDeDedo();
        AplicarRotacion();
    }

    void DetectarMovimientoDeDedo()
    {
        if (Input.touchCount > 0)
        {
            foreach (Touch t in Input.touches)
            {
                // Si el dedo recien tocó, verificar que NO toque UI
                if (t.phase == TouchPhase.Began)
                {
                    if (!EventSystem.current.IsPointerOverGameObject(t.fingerId))
                    {
                        fingerID = t.fingerId; // Este dedo controla la cámara
                    }
                }

                // Si este dedo es el que controla la cámara
                if (t.fingerId == fingerID)
                {
                    if (t.phase == TouchPhase.Moved)
                    {
                        lookInput = t.deltaPosition * sensibilidad;
                    }
                    else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
                    {
                        fingerID = -1;
                        lookInput = Vector2.zero;
                    }
                }
            }
        }
        else
        {
            fingerID = -1;
            lookInput = Vector2.zero;
        }
    }

    void AplicarRotacion()
    {
        rotX -= lookInput.y;
        rotY += lookInput.x;

        // Limitar eje vertical como en PUBG
        rotX = Mathf.Clamp(rotX, minY, maxY);

        transform.localRotation = Quaternion.Euler(rotX, rotY, 0);
    }
}

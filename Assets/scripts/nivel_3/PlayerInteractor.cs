using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private Camera cam;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactMask = ~0;

    [Header("UI")]
    [SerializeField] private Button interactButton;   // Botón del Canvas

    private IInteractable current;

    private void Start()
    {
        if (cam == null) cam = Camera.main;
        interactButton.gameObject.SetActive(false);
        interactButton.onClick.AddListener(OnButtonClicked);
    }

    private void Update()
    {
        current = null;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactMask))
        {
            var interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null && interactable.CanInteract)
                current = interactable;
        }

        interactButton.gameObject.SetActive(current != null);
    }

    private void OnButtonClicked()
    {
        if (current != null) current.Interact();
    }
}
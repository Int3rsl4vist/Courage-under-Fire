using UnityEngine;
using UnityEngine.UI;


public class PlayerInteraction : MonoBehaviour
{
    public float interactionDistance = 3f;
    public GameObject interactionText;

    private KeyCode _interactionKey = KeyCode.F;
    private Camera _cam;

    private void Start()
    {
        if (_cam == null)
            _cam = Camera.main;
        if (_cam == null)
            Debug.LogError("CODE_ERROR: No Camera with the 'MainCamera' tag found");
    }

    private void Update()
    {
        Ray ray = new(transform.position, transform.forward);
        bool hitInteractableObject = false;
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            if(hit.collider.TryGetComponent(out ObjectController interactableObject))
            {
                hitInteractableObject = true;
                if (Input.GetKeyDown(_interactionKey))
                    interactableObject.Interact();
            }
        }
        if(interactionText != null)
        {
            interactionText.SetActive(hitInteractableObject);
        }
    }
}
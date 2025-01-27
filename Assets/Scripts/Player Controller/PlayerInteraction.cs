using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionDistance;
    public GameObject interactionText;
    public string doorOpenAnimationName, doorCloseAnimationName;
    protected KeyCode interKey;
    private void Start()
    {
        interKey = KeyCode.F;
        //interactionText = $"Press{interKey} to interact";
    }
    private void Update()
    {
        Ray ray = new(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            if (hit.collider.gameObject.CompareTag("Door"))
            {
                GameObject doorMover = hit.collider.transform.parent.parent.gameObject;
                Animator doorAnimator = doorMover.GetComponent<Animator>();
                interactionText.SetActive(true);
                if (Input.GetKeyDown(interKey))
                {
                    if (doorAnimator.GetCurrentAnimatorStateInfo(0).IsName(doorOpenAnimationName))
                    {
                        doorAnimator.ResetTrigger("Open");
                        doorAnimator.SetTrigger("Close");
                    }
                    if (doorAnimator.GetCurrentAnimatorStateInfo(0).IsName(doorCloseAnimationName))
                    {
                        doorAnimator.ResetTrigger("Close");
                        doorAnimator.SetTrigger("Open");
                    }
                }
            }
            else interactionText.SetActive(false);
        }
    }
}
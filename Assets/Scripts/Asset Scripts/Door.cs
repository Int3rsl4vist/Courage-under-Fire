using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public float interactionDistance;
    public GameObject interactionText;
    public string doorOpenAnimationName, doorCloseAnimationName;
    bool unlocked = true;

    private void Update()
    {
        Ray ray = new(transform.position, transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.gameObject.tag == "Door")
            {
                if (unlocked)
                {
                    Debug.Log("Ray hit");
                    GameObject doorMover = hit.collider.transform.parent.parent.gameObject;
                    Animator doorAnimator = doorMover.GetComponent<Animator>();
                    interactionText.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        Debug.Log("Interaction key pressed");
                        if (doorAnimator.GetCurrentAnimatorStateInfo(0).IsName(doorOpenAnimationName))
                        {
                            Debug.Log("Opening door");
                            doorAnimator.ResetTrigger("Open");
                            doorAnimator.SetTrigger("Close");
                        }
                        if (doorAnimator.GetCurrentAnimatorStateInfo(0).IsName(doorCloseAnimationName))
                        {
                            Debug.Log("Closing door");
                            doorAnimator.ResetTrigger("Close");
                            doorAnimator.SetTrigger("Open");
                        }
                    }
                }
                unlocked = false;
            }
            else
                interactionText.SetActive(false);
        }
    }
}
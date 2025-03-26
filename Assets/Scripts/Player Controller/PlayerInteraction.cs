using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;
using UnityEngine.UI;


public class PlayerInteraction : MonoBehaviour
{
    public float interactionDistance;
    public GameObject interactionText;

    public List<string> animationNames = new() { "DoorOpen", "DoorClose"};

    protected KeyCode interKey;

    private void Start()
    {
        interKey = KeyCode.F;
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
                    if (doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("DoorOpen"))
                    {
                        doorAnimator.ResetTrigger("Open");
                        doorAnimator.SetTrigger("Close");
                    }
                    else if (doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("DoorClose"))
                    {
                        doorAnimator.ResetTrigger("Close");
                        doorAnimator.SetTrigger("Open");
                    }
                }
            }
            /*else if (hit.collider.gameObject.CompareTag("Weapon"))
            {
                interactionText.SetActive(true);
                if(Input.GetKeyDown(interKey))
                {
                    Destroy(hit.collider.gameObject);
                    
                }
            }*/
            else if (hit.collider.gameObject.CompareTag("AmmoCrate"))
            {
                GameObject mover = hit.collider.transform.parent.gameObject;
                Animator anim = mover.GetComponent<Animator>();
                interactionText.SetActive(true);
                if (Input.GetKeyDown(interKey))
                {
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName("CrateOpen"))
                    {
                        anim.ResetTrigger("Open");
                        anim.SetTrigger("Close");
                    }
                    else if (anim.GetCurrentAnimatorStateInfo(0).IsName("CrateClose"))
                    {
                        anim.ResetTrigger("Close");
                        anim.SetTrigger("Open");
                    }

                    // Debug all animations in the dictionary
                    foreach (string animation in animationNames)
                    {
                        Debug.Log($"Animation: {animation}");
                    }
                }
            }
            else
            {
                interactionText.SetActive(false);
            }
        }
    }
}
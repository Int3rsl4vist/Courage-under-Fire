using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject interactionText;
    public float interactionDistance;
    private KeyCode interKey;
    private Animator anim;
    public Transform cam;

    void Start()
    {
        interKey = KeyCode.F;
        //base.Start(); // Call the base Start method
        anim = GetComponent<Animator>(); // Get the Animator component
        Debug.Log("Starting");
    }

    void Update()
    {
        Debug.Log($"Text: {interactionText} | Distance: {interactionDistance}");
        Ray ray = new(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            if (hit.collider.gameObject.CompareTag("Door"))
            {
                interactionText.SetActive(true);

                if (Input.GetKeyDown(interKey))
                {
                    HandleAnimation();
                }
            }
            else
            {
                interactionText.SetActive(false);
            }
        }
        else
        {
            interactionText.SetActive(false);
        }
    }

    private void HandleAnimation()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("DoorOpen"))
        {
            anim.ResetTrigger("Open");
            anim.SetTrigger("Close");
            Debug.Log("Door closing!");
        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("DoorClose"))
        {
            anim.ResetTrigger("Close");
            anim.SetTrigger("Open");
            Debug.Log("Door opening!");
        }
    }
}
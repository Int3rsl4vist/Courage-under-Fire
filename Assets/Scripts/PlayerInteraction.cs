using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public float pickupRange = 3.0f;
    public LayerMask weaponLayer;
    public GameObject pickupPromptUI;
    private void Update()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, pickupRange, weaponLayer))
        {
            if (hit.collider.TryGetComponent<LMG>(out var weapon))
            {
                pickupPromptUI.SetActive(true);
                pickupPromptUI.GetComponent<Text>().text = "Press 'E' to pick up " + weapon.gameObject.name;

                if (Input.GetButtonDown("PickUp"))
                {
                    PickUpWeapon(weapon);
                }
            }
            else
            {
                pickupPromptUI.SetActive(false);
            }
        }
        else
        {
            pickupPromptUI.SetActive(false);
        }
    }

    private void PickUpWeapon(Weapon weapon)
    {
        weapon.PickUp();
        Debug.Log("Picked up weapon: " + weapon.gameObject.name);
    }
}
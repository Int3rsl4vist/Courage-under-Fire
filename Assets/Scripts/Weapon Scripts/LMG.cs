using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class LMG : Weapon
{
    public WeaponData weaponData;
    public BoxCollider Collider;

   /* private void Start()
    {
        /*weaponData = new WeaponData();
        {
            weaponData.ammo = this.ammo;                  
            weaponData.magazineSize = this.magazineSize;  
            weaponData.magazineCount = this.magazineCount; 
            weaponData.isEquipped = this.isEquipped;
        }        
        ammo = magazineSize;
        //cldr = GetComponent<BoxCollider>();
    }*/
    
    /*public override void PickUp()
    {
        weaponData.isEquipped = true;
        this.gameObject.SetActive(false);
        Debug.Log("Weapon picked up: " + gameObject.name);
    }*/
}
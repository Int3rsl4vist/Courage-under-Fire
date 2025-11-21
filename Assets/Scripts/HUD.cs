using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    public Weapon currentWeapon;
    public TMP_Text ammoData;
    public TMP_Text weaponName;
    void Update()
    {
        if(currentWeapon == null)
        {
            weaponName.SetText("Weapon");
            ammoData.SetText("X / X");
        }
        else
        {
            ammoData.SetText($"{currentWeapon.ammo} / {currentWeapon.magazineCount * currentWeapon.magazineSize}");
            weaponName.SetText(currentWeapon.name);
        }
    }
}

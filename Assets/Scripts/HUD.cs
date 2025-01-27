using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    public Weapon currentWeapon;
    public TMP_Text ammoData;
    public TMP_Text weaponName;
    // Update is called once per frame
    void Update()
    {
        ammoData.SetText($"{currentWeapon.ammo} / {currentWeapon.magazineCount * currentWeapon.magazineSize}");
        weaponName.SetText(currentWeapon.name);
    }
}

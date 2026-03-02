using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [Header("Weapon Panel:")]
    public Image weaponIcon;
    public TextMeshProUGUI ammoText;

    public Color fullAmmoColor = Color.white;
    public Color lowAmmoColor = Color.red;

    private void Awake()
    {
        Instance = this;
    }
    public void UpdateAmmoUI(Weapon weapon)
    {
        if (weapon.weaponIcon != null)
        {
            weaponIcon.sprite = weapon.weaponIcon;
            weaponIcon.enabled = true;
            weaponIcon.preserveAspect = true;
        }
        else
            weaponIcon.enabled = false;

        ammoText.text = $"{weapon.curAmmo} <size=60%>| {weapon.magazinesLeft}</size>";

        if (weapon.curAmmo <= weapon.magazineSize * 0.25f)
            ammoText.color = lowAmmoColor;
        else
            ammoText.color = fullAmmoColor;
    }
    public void ClearWeaponUI()
    {
        if(weaponIcon != null)
        {
            weaponIcon.sprite = null;
            weaponIcon.enabled = false;
        }
        if(ammoText != null)
        {
            ammoText.text = "";
        }
    }
}

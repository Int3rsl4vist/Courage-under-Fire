using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Equipment:")]
    public Weapon activeWeapon;

    private void Update()
    {
        if(activeWeapon == null) return;

        if (activeWeapon.isAutomatic)
        {
            if (Input.GetButton("Fire"))
                activeWeapon.TryShoot();
        }
        else
        {
            if (Input.GetButtonDown("Fire"))
                activeWeapon.TryShoot();
        }
        
        if(Input.GetButtonDown("Reload"))
            activeWeapon.Reload();
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        if (activeWeapon != null)
            activeWeapon.OnAmmoChange -= UpdateAmmoHUD;
        
        activeWeapon = newWeapon;
        activeWeapon.OnAmmoChange += UpdateAmmoHUD;

        UpdateAmmoHUD();
    }
    public void DropWeapon()
    {
        if (activeWeapon != null)
        {
            activeWeapon.OnAmmoChange -= UpdateAmmoHUD;
            activeWeapon = null;
        }
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.ClearWeaponUI();
        }
    }
    void UpdateAmmoHUD()
    {
        if (activeWeapon != null && HUDManager.Instance != null)
            HUDManager.Instance.UpdateAmmoUI(activeWeapon);
    }
}

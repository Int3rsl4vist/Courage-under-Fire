using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(PlayerInput))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Equipment:")]
    public Weapon activeWeapon;

    private PlayerInput _playerInput;
    private InputAction _fireAction;
    private InputAction _reloadAction;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _fireAction = _playerInput.actions["Fire"];
        _reloadAction = _playerInput.actions["Reload"];
    }
    private void Update()
    {
        if(activeWeapon == null) return;

        if (activeWeapon.isAutomatic)
        {
            if (_fireAction.ReadValue<float>() > 0.1f)
                activeWeapon.TryShoot();
        }
        else
        {
            if (_fireAction.WasPressedThisFrame())
                activeWeapon.TryShoot();
        }
        
        if(_reloadAction.WasPressedThisFrame())
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

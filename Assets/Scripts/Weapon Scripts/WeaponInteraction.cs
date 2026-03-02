using UnityEngine;

public class WeaponInteraction : MonoBehaviour
{
    [Header("Settings:")]
    public float interactionRange = 3f;
    public Transform weaponHolder;
    public LayerMask weaponLayer;

    [Header("Keys:")]
    public KeyCode pickupKey = KeyCode.F;
    public KeyCode dropKey = KeyCode.G;

    [Header("Dual Camera Settings:")]
    public string weaponLayerName = "Weapon";
    public string worldLayerName = "Default";

    [Header("References:")]
    public PlayerCombat playerCombat;

    private Weapon curWeapon;

    private void Start()
    {
        if (playerCombat == null)
            playerCombat = GetComponent<PlayerCombat>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(dropKey) && curWeapon != null)
            DropWeapon();

        if (Input.GetKeyDown(pickupKey))
            TryPickupWeapon();
    }

    private void TryPickupWeapon()
    {
        Ray ray = new(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, weaponLayer))
        {
            Weapon weaponOnGround = hit.transform.GetComponentInParent<Weapon>();

            if (weaponOnGround != null)
            {
                if (curWeapon != null) DropWeapon();
                EquipWeapon(weaponOnGround);
            }
        }
    }

    private void EquipWeapon(Weapon newWeapon)
    {
        curWeapon = newWeapon;
        newWeapon.transform.SetParent(weaponHolder);
        newWeapon.OnEquip();
        ChangeLayerRecursive(newWeapon.gameObject, LayerMask.NameToLayer(weaponLayerName));

        if (newWeapon.handPosition != Vector3.zero)
            newWeapon.transform.SetLocalPositionAndRotation(newWeapon.handPosition, Quaternion.Euler(newWeapon.handRotation));
        else
            newWeapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        if (playerCombat != null)
            playerCombat.EquipWeapon(newWeapon);
        else
            Debug.LogError("CODE_ERROR: Missing PlayerCombat script! Shooting not possible");
    }

    private void DropWeapon()
    {
        if (curWeapon == null) return;

        curWeapon.OnDrop();
        curWeapon.transform.parent = null;
        ChangeLayerRecursive(curWeapon.gameObject, LayerMask.NameToLayer(worldLayerName));

        if (curWeapon.rb != null)
        {
            curWeapon.rb.isKinematic = false;
            curWeapon.rb.AddForce(Camera.main.transform.forward * 3f + Vector3.up * 2f, ForceMode.Impulse);
            curWeapon.rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
        }
        if (playerCombat != null)
        {
            playerCombat.DropWeapon();
        }

        curWeapon = null;
    }

    void ChangeLayerRecursive(GameObject obj, int newLayer)
    {
        if (newLayer == -1) return;

        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            ChangeLayerRecursive(child.gameObject, newLayer);
        }
    }
}
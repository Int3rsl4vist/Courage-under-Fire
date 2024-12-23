using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class LMG : Weapon
{
    public WeaponData weaponData;
    private bool isShooting = false;

    private void Start()
    {
        weaponData = new WeaponData();
        {
            weaponData.ammo = this.ammo;                  
            weaponData.magazineSize = this.magazineSize;  
            weaponData.magazineCount = this.magazineCount; 
            weaponData.isEquipped = this.isEquipped;
        }        
        ammo = magazineSize;
        fireRate /= 60f;
    }
    private void Update()
    {
        HandleInput();

    }
    private void HandleInput()
    {
        if (Input.GetMouseButton(0) && ammo > 0)
            Attack();
        if (Input.GetButtonDown("Reload"))
            Reload();
        if (Input.GetButtonDown("Drop"))
            Drop();
    }

    public override void Attack()
    {
        if(ammo == 0)
        {
            Debug.Log("Out of ammo");
            return;
        }
        if (isShooting) 
            return;
        /*if (!isEquipped)
            return;*/
        StartCoroutine(AttackCoroutine());
    }
    private IEnumerator AttackCoroutine()
    {
        isShooting = true;
        var bullet = Instantiate(bulletPrefab, bulletSpawner.position, bulletSpawner.rotation);
        if (bullet.TryGetComponent<Rigidbody>(out var bulletRb))
        {
            bulletRb.AddForce(bulletSpawner.forward * (bulletSpeed/2), ForceMode.Impulse);
        }
        Destroy(bullet, 2.5f);
        ammo--;
        Debug.Log("LMG attack");
        yield return new WaitForSeconds(1f/fireRate);
        isShooting = false;
    }
    public override void Reload()
    {
        if (magazineCount == 0)
        {
            Debug.Log("Out of magazines");
            return;
        }
        StartCoroutine(ReloadCoroutine());
    }
    private IEnumerator ReloadCoroutine()
    {
        magazineCount--;
        yield return new WaitForSeconds(1f);
        ammo = magazineSize;
        
    }
    public override void Drop()
    {
        this.gameObject.SetActive(false);

        GameObject droppedWeapon = Instantiate(gameObject, transform.position, transform.rotation);

        droppedWeapon.SetActive(true);
        droppedWeapon.AddComponent<Rigidbody>();
        droppedWeapon.AddComponent<Collider>().isTrigger = true;

        LMG weaponScript = droppedWeapon.GetComponent<LMG>();

        if (droppedWeapon.TryGetComponent<Rigidbody>(out var weaponRb))
        {
            weaponRb.useGravity = true;
        }
        if (weaponScript != null)
        {
            weaponScript.isEquipped = false;
        }
    }
    public override void PickUp()
    {
        weaponData.isEquipped = true;
        this.gameObject.SetActive(false);
        Debug.Log("Weapon picked up: " + gameObject.name);
    }
}
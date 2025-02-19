using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class Weapon : MonoBehaviour, IDataPersistance
{
    public bool isEquipped;
    public int ammo;
    public int magazineSize;
    public int magazineCount;
    public Transform bulletSpawner;
    public GameObject bulletPrefab;
    public float fireRate;
    public float bulletSpeed;
    public GameObject pickupPromptUI;
    public float reloadSpeed;
    public bool isShooting = false;
    public bool readyToShoot = true;
    protected enum WeaponType { Automatic, SemiAutomatic, SingleShot, Melee}

    private void Start()
    {
        fireRate /= 60f;
        ammo = magazineSize;
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
    public virtual void Attack()
    {
        if (ammo == 0)
        {
            Debug.Log("Out of ammo");
            return;
        }
        if (isShooting)
            return;
        /*if (!isEquipped)
            return;*/
        if (readyToShoot)
            StartCoroutine(AttackCoroutine());
    }
    public IEnumerator AttackCoroutine()
    {
        isShooting = true;
        var bullet = Instantiate(bulletPrefab, bulletSpawner.position, bulletSpawner.rotation);
        if (bullet.TryGetComponent<Rigidbody>(out var bulletRb))
        {
            bulletRb.AddForce(bulletSpawner.forward * (bulletSpeed / 2), ForceMode.Impulse);
        }
        Destroy(bullet, 2.5f);
        ammo--;
        Debug.Log($"LMG attack");
        yield return new WaitForSeconds(1 / fireRate);
        isShooting = false;
    }
    public virtual void Reload()
    {
        if (magazineCount == 0)
        {
            Debug.Log("Out of magazines");
            return;
        }
        StartCoroutine(ReloadCoroutine());
    }
    public IEnumerator ReloadCoroutine()
    {
        readyToShoot = false;
        magazineCount--;
        yield return new WaitForSeconds(1f);
        ammo = magazineSize;
        readyToShoot = true;
    }
    public virtual void Drop()
    {
        this.gameObject.SetActive(false);

        GameObject droppedWeapon = Instantiate(gameObject, transform.position, transform.rotation);

        droppedWeapon.SetActive(true);
        droppedWeapon.AddComponent<Rigidbody>();
        droppedWeapon.GetComponent<BoxCollider>().enabled = true;

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
    public virtual void PickUp()
    {
        Debug.Log("Picking up " + this.GetType().Name);
    }

    public void LoadData(GameData data)
    {
        this.ammo = data.ammo;
    }

    public void SaveData(ref GameData data)
    {
        data.ammo = this.ammo;
    }
}
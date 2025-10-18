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
    public AudioSource audio;
    public AudioClip equipClip;
    public AudioClip shotClip;
    public AudioClip reloadClip;
    protected enum WeaponType { Automatic, SemiAutomatic, SingleShot, Melee}

    private void Start()
    {
        fireRate /= 60f;
        ammo = magazineSize;
        audio = GetComponent<AudioSource>();
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
        audio.PlayOneShot(shotClip);
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit))
        {
            Transform hitObject = hit.transform;
            Debug.Log($"Ray hit: {hit.transform.name}");
            if (hitObject.CompareTag("Destroyable"))
                Destroy(hitObject.gameObject);
        }
        ammo--;
        //Debug.Log($"Shooting");
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
        audio.PlayOneShot(reloadClip);
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
        droppedWeapon.layer = 7;
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
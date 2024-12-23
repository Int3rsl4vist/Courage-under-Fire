using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
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
    protected enum WeaponType { Automatic, SemiAutomatic, SingleShot, Melee}

    private void Start()
    {
        fireRate /= 60f;
    }
    public virtual void Aim()
    {
        Debug.Log("Aiming with " + this.GetType().Name);
    }
    public virtual void Attack()
    {
        Debug.Log("Attacking with " + this.GetType().Name);
    }
    public virtual void Reload()
    {
        Debug.Log("Reloading " + this.GetType().Name);
    }
    public virtual void Drop()
    {
        Debug.Log("Dropping " + this.GetType().Name);
    }
    public virtual void PickUp()
    {
        Debug.Log("Picking up " + this.GetType().Name);
    }
}
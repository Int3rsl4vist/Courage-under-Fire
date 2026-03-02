using System;
using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBehaviour, IDataPersistance
{
    [Header("Weapon Stats:")]
    public string weaponName = "Weapon";
    public int curAmmo;
    public int magazineSize = 12;
    public int magazinesLeft = 3;

    [Header("Shooting Settings:")]
    public float fireRate = 600f;
    public float range = 100f;
    public float damage = 20f;
    public float reloadTime = 2f;
    public bool isAutomatic = false;

    [Header("Stealth & Noise:")]
    public float noiseRadius = 30f;
    public LayerMask enemyLayer;

    [Header("Audio:")]
    public AudioSource weaponAudio;
    public AudioClip shotClip;
    public AudioClip reloadClip;
    public AudioClip emptyClickClip;

    [Header("Visuals:")]
    public Renderer muzzleFlashMesh;
    public float flashDuration = 0.05f;
    public GameObject impactPrefab;
    public float impactForce = 100f;

    [Header("Save System ID:")]
    [Tooltip("Unique ID for saving ('Pistol_01', 'Rifle_01', etc.)")]
    public string weaponID;

    [Header("Positioning:")]
    public Vector3 handPosition;
    public Vector3 handRotation;

    [Header("ADS Positioning:")]
    public Vector3 aimPosition;
    public Vector3 aimRotation;
    public float aimSpeed = 8f;

    [Header("Physics:")]
    public Rigidbody rb;
    public Collider[] weaponColliders;

    [Header("UI:")]
    public Sprite weaponIcon;

    [Header("Recoil Settings:")]
    public float recoilX = 2f;
    public float recoilY = 2f;
    public float recoilZ = 0.35f;

    protected bool isReloading = false;
    protected float nextTimeToFire = 0f;

    public event Action OnAmmoChange;

    private void Awake()
    {
        if(rb ==  null) rb = GetComponent<Rigidbody>();
        weaponColliders ??= GetComponentsInChildren<Collider>(true);

        Debug.Log($"CODE_LOG: Found {weaponColliders.Length} colliders on {gameObject.name} weapon");
    }
    private void Start()
    {
        curAmmo = magazineSize;
        if(weaponAudio ==  null)
            weaponAudio = GetComponent<AudioSource>();
    }
    public virtual void TryShoot()
    {
        if(isReloading) return;
        
        if(curAmmo <= 0)
        {
            if (Input.GetButtonDown("Fire"))
                PlaySound(emptyClickClip);
            return;
        }

        if(Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + (60f / fireRate);
            Shoot();
        }
    }
    protected virtual void Shoot()
    {
        curAmmo--;
        if (WeaponSway.Instance != null)
        {
            WeaponSway.Instance.AddRecoil(recoilX, recoilY, recoilZ);
        }
        OnAmmoChange?.Invoke();
        PlaySound(shotClip);
        if (muzzleFlashMesh != null)
            StartCoroutine(FlashEffect());
        MakeNoise();

        Ray ray = new(Camera.main.transform.position, Camera.main.transform.forward);
        if(Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Debug.Log($"CODE_LOG: Hit: {hit.transform.name}");

            IDamageable target = hit.transform.GetComponentInParent<IDamageable>();
            target?.TakeDamage(damage);

            if(hit.rigidbody != null)
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            if (hit.transform.CompareTag("Destroyable"))
                Destroy(hit.transform.gameObject);
            if(impactPrefab != null)
            {
                GameObject impact = Instantiate(impactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                impact.transform.position += impact.transform.forward * 0.01f;
                impact.transform.parent = hit.transform;
                Destroy(impact, 16f);
            }
        }
    }
    private IEnumerator FlashEffect()
    {
        muzzleFlashMesh.enabled = true;
        //muzzleFlashMesh.transform.eulerAngles = new(0, Random.Range(0, 360), 0);

        yield return new WaitForSeconds(flashDuration);

        muzzleFlashMesh.enabled = false;
    }
    void MakeNoise()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, noiseRadius, enemyLayer);

        foreach(var enemy in enemiesInRange)
        {
            // WIP: enemy.GetComponent<EnemyAI>().Alert(transform.position);
            Debug.Log($"CODE_LOG: Enemy: {enemy.name} alerted");
        }
    }
    public void Reload()
    {
        if(isReloading || curAmmo == magazineSize || magazinesLeft <= 0) return;

        StartCoroutine(ReloadCoroutine());
    }
    IEnumerator ReloadCoroutine()
    {
        Debug.Log("CODE_LOG: Reloading");

        isReloading = true;
        PlaySound(reloadClip);

        yield return new WaitForSeconds(reloadTime);

        magazinesLeft--;
        curAmmo = magazineSize;
        isReloading = false;

        OnAmmoChange?.Invoke();

        Debug.Log("CODE_LOG: Reload complete");
    }
    void PlaySound(AudioClip clip)
    {
        if(clip != null && weaponAudio != null)
            weaponAudio.PlayOneShot(clip);
    }
    public void OnEquip()
    {
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        foreach (var col in weaponColliders)
            col.enabled = false;
    }
    public void OnDrop()
    {
        rb.isKinematic = false;
        transform.parent = null;

        foreach (var col in weaponColliders)
            col.enabled = true;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, noiseRadius);
    }
    public void LoadData(GameData data)
    {
        if (data.weaponsAmmo.ContainsKey(weaponID))
        {
            this.curAmmo = data.weaponsAmmo[weaponID];
        }
        else
        {
            Debug.Log($"CODE_LOG: Weapon {weaponID} not found");
        }
    }
    public void SaveData(ref GameData data)
    {
        if (data.weaponsAmmo.ContainsKey(weaponID))
        {
            data.weaponsAmmo[weaponID] = this.curAmmo;
        }
        else
        {
            data.weaponsAmmo.Add(weaponID, this.curAmmo);
        }
    }
}
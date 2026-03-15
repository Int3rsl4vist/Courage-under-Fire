using UnityEngine;

public class Bazooka : Weapon
{
    [Header("Bazooka settings:")]
    public GameObject rocketPrefab;
    [Tooltip("Empty object on the end of the barrel, where the rocket will be instantiated")]
    public Transform firePoint;

    protected override void Shoot()
    {
        curAmmo--;
        if (WeaponSway.Instance != null)
            WeaponSway.Instance.AddRecoil(recoilX, recoilY, recoilZ);
        TriggerAmmoChange();
        PlaySound(shotClip);
        MakeNoise();
        if (muzzleFlashMesh != null)
            StartCoroutine(FlashEffect());
        if (rocketPrefab != null && firePoint != null)
        {
            Ray ray = new(Camera.main.transform.position, Camera.main.transform.forward);
            Vector3 targetPoint;
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, hitLayers))
                targetPoint = hit.point;
            else
                targetPoint = ray.GetPoint(1000f);
            Vector3 direction = targetPoint - firePoint.position;

            GameObject currentRocket = Instantiate(rocketPrefab, firePoint.position, Quaternion.identity);
            currentRocket.transform.forward = direction.normalized;
        }
    }
}

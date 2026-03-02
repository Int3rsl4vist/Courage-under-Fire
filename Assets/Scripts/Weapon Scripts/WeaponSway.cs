using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public static WeaponSway Instance;
    
    [Header("Sway Settings")]
    public float intensity = 1f;
    public float smooth = 10f;
    public float maxSway = 5f;

    [Header("Recoil Settings")]
    public float recoilReturnSpeed = 5f;
    public float recoilSnappiness = 10f;

    private PlayerCombat playerCombat;

    private Vector3 originPosition;
    private Quaternion originRotation;

    private Quaternion currentBaseRotation;

    private Vector3 currentRecoilRot;
    private Vector3 targetRecoilRot;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        originPosition = transform.localPosition;
        originRotation = transform.localRotation;
        currentBaseRotation = originRotation;

        playerCombat = GetComponentInParent<PlayerCombat>();
    }

    private void Update()
    {
        UpdateSwayAndRecoil();
        UpdateADS();
    }

    void UpdateADS()
    {
        bool isAiming = (playerCombat != null && playerCombat.activeWeapon != null && Input.GetButton("Aim"));
        float speed = (playerCombat != null && playerCombat.activeWeapon != null) ? playerCombat.activeWeapon.aimSpeed : 8f;

        Vector3 targetPosition = isAiming ? playerCombat.activeWeapon.aimPosition : originPosition;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * speed);

        Quaternion targetRotation = isAiming ? Quaternion.Euler(playerCombat.activeWeapon.aimRotation) : originRotation;

        currentBaseRotation = Quaternion.Slerp(currentBaseRotation, targetRotation, Time.deltaTime * speed);
    }

    void UpdateSwayAndRecoil()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float currentIntensity = intensity;

        if(Input.GetButton("Aim"))
            currentIntensity *= 0.2f;

        Quaternion rotationX = Quaternion.AngleAxis(-mouseY * currentIntensity, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX * currentIntensity, Vector3.up);

        Quaternion targetSway = currentBaseRotation * rotationX * rotationY;

        targetRecoilRot = Vector3.Lerp(targetRecoilRot, Vector3.zero, recoilReturnSpeed * Time.deltaTime);
        currentRecoilRot = Vector3.Slerp(currentRecoilRot, targetRecoilRot, recoilSnappiness * Time.fixedDeltaTime);
        Quaternion recoilQuaternion = Quaternion.Euler(currentRecoilRot);

        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetSway * recoilQuaternion, Time.deltaTime * smooth);
    }

    public void AddRecoil(float x, float y, float z)
    {
        float multiplier = Input.GetButton("Aim") ? .5f : 1f;
        targetRecoilRot += new Vector3(-x, Random.Range(-y, y), Random.Range(-z, z)) * multiplier;
    }
}
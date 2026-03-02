using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [Header("Settings:")]
    public bool enableHeadBob = true;

    [Header("Movement:")]
    public float walkingBobbingSpeed = 14f;
    public float bobbingAmount = 0.05f;
    public float smooth = 10f;

    private float defaultPosY = 0;
    private float timer = 0;

    private void Start()
    {
        defaultPosY = transform.localPosition.y;
    }
    private void Update()
    {
        if (!enableHeadBob) return;

        CheckMotion();
        ResetPosition();
    }
    private void CheckMotion()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");
        Vector3 movementInput = new(hor, 0, ver);

        if (movementInput.magnitude > .1f)
        {
            timer += Time.deltaTime * walkingBobbingSpeed;
            float newX = Mathf.Cos(timer / 2) * bobbingAmount;
            float newY = defaultPosY + Mathf.Sin(timer) * bobbingAmount;
            transform.localPosition = new(newX, newY, transform.localPosition.z);
        }
        else
            timer = 0;
    }
    private void ResetPosition()
    {
        if(Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
        {
            Vector3 targetPosition = new(transform.localPosition.x, defaultPosY, transform.localPosition.z);
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * smooth);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPCam : MonoBehaviour
{
    public float sensitivityX;
    public float sensitivityY;

    public Transform playerRotation;

    protected float rotationX;
    protected float rotationY;

    internal void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    internal void Update()
    {
        // Mouse Input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivityX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivityY;

        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        // Camera and Player Rotation
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
        playerRotation.rotation = Quaternion.Euler(0, rotationY, 0);

    }
}

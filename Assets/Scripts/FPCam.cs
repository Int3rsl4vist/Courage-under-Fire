using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPCam : MonoBehaviour
{
    public float sensitivityX;
    public float sensitivityY;

    public Transform playerRotation;
    public Transform playerModel;

    protected float rotationX;
    protected float rotationY;

    internal void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    internal void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivityY;
        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        //Debug.Log($"{rotationX} | {rotationY}");

        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
        playerRotation.rotation = Quaternion.Euler(0, rotationY, 0);
        playerModel.rotation = Quaternion.Euler(0, rotationY, 0);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Movement
    private Dictionary<Stance, float[]> moveSpeeds = new Dictionary<Stance, float[]>()
    {
        { Stance.Standing, new float[] { 2.5f, 5f, 7.5f, 9f } },
        { Stance.Crouching, new float[] { 2f, 4f, 6f, 7f } },
        { Stance.Lying, new float[] { 1f, 1.5f, 2f, 2.5f } }
    };
    private Vector3 moveDirection;
    public int speedIndicator = 1;
    
    // Stance
    private enum Stance { Standing, Crouching, Lying }
    private Stance currentStance = Stance.Standing;
    public bool isCrouching = false;
    public bool isLying = false;
    private float transitionSpeed = 4f;
    
    // Rotation
    private Quaternion rotation = Quaternion.identity;
    private float rotationX = 0f;
    private float rotationY = 0f;
    
    // Scale
    private Vector3 originalScale;
    private Vector3 scale = Vector3.one;

    // Camera
    private float cameraSensitivity = 5f;
    private float rotationSpeed = 5f;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("CameraController component not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");
        rotationX += mouseX * cameraSensitivity;
        rotationY += mouseY * cameraSensitivity;
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        transform.RotateAround(transform.position, Vector3.up, mouseX * cameraSensitivity);
        transform.RotateAround(transform.position, transform.right, -mouseY * cameraSensitivity);
        transform.eulerAngles = new Vector3(-rotationY, rotationX, 0f);
        HandleInput();
        UpdateStance();
        UpdateMovement();
    }
    private void HandleInput()
    {
        if (Input.GetButtonDown("Crouch"))
            currentStance = currentStance == Stance.Crouching ? Stance.Standing : Stance.Crouching;
        if (Input.GetButtonDown("Prone"))
            currentStance = currentStance == Stance.Lying ? Stance.Standing : Stance.Lying;
        if (Input.GetButtonDown("Faster"))
            speedIndicator++;
        else if (Input.GetButtonDown("Slower"))
            speedIndicator--;
        speedIndicator = Mathf.Clamp(speedIndicator, 0, 3);
    }
    private void UpdateStance()
    {
        switch (currentStance)
        {
            case Stance.Crouching:
                UpdateCrouchState();
                break;
            case Stance.Lying:
                UpdateLyingState();
                break;
            default:
                UpdateStandingState();
                break;
        }
    }
    private void UpdateCrouchState()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(originalScale.x, originalScale.y * 0.5f, originalScale.z), transitionSpeed * Time.deltaTime);
        transform.rotation = Quaternion.identity;
        isCrouching = true;
        isLying = false;
    }
    private void UpdateLyingState()
    {
        transform.localScale = originalScale;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 90), transitionSpeed * Time.deltaTime);
        isCrouching = false;
        isLying = true;
    }
    private void UpdateStandingState()
    {
        transform.localScale = originalScale;
        transform.rotation = Quaternion.identity;
        isCrouching = false;
        isLying = false;
    }
    private float GetMoveSpeed(Stance stance)
    {
        return moveSpeeds[stance][Mathf.Clamp(speedIndicator, 0, moveSpeeds[stance].Length - 1)];
    }
    private void UpdateMovement()
    {
        moveDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            moveDirection += mainCamera.transform.TransformDirection(Vector3.forward);
        if (Input.GetKey(KeyCode.S))
            moveDirection += mainCamera.transform.TransformDirection(Vector3.back);
        if (Input.GetKey(KeyCode.A))
            moveDirection += mainCamera.transform.TransformDirection(Vector3.left);
        if (Input.GetKey(KeyCode.D))
            moveDirection += mainCamera.transform.TransformDirection(Vector3.right);
        moveDirection.y = 0f;
        moveDirection = moveDirection.normalized;

        transform.Translate(GetMoveSpeed(currentStance) * Time.deltaTime * moveDirection, Space.World);
    }
    void LateUpdate()
    {
        Vector3 cameraDirection = Camera.main.transform.forward;
        Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(cameraDirection, Vector3.up), rotationSpeed * Time.deltaTime);
    }
}
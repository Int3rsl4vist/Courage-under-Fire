using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;

    [Header("Stance")]
    public bool isCrouching;
    public bool isLying;
    protected enum Stance { Standing, Crouching, Lying }
    protected Stance currentStance = Stance.Standing;
    public int speedIndicator = 1;
    public float transitionSpeed = 4f;

    [Header("Player state")]
    public Vector3 originalScale;
    public float playerHeight = 2f;
    public LayerMask groundLayer;
    public bool isGrounded;
    private bool readyToJump;
    public bool onSlope;

    // Player movement references
    public Transform playerRotation;
    public Transform playerModel;
    private Rigidbody rb;

    // Input variables
    private float horInput = 0;
    private float verInput = 0;
    private Vector3 moveDirection;

    // Speed settings for different stances
    private readonly Dictionary<Stance, float[]> moveSpeeds = new()
    {
        { Stance.Standing, new float[] { 2.5f, 5f, 7.5f, 9f } },
        { Stance.Crouching, new float[] { 2f, 4f, 6f, 7f } },
        { Stance.Lying, new float[] { 1f, 1.5f, 2f, 2.5f } }
    };

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalScale = transform.localScale;
        readyToJump = true;
        currentStance = Stance.Standing;
        speedIndicator = 1;
        moveSpeed = 5f;
    }

    private void Update()
    {
        CheckGroundStatus();
        HandleInput();
        UpdateStance();
        CheckSlope();
        MovePlayer();
    }
    private void CheckGroundStatus()
    {
        isGrounded = Physics.CheckSphere(transform.position, playerHeight / 2 + 0.25f, groundLayer);
        rb.drag = isGrounded ? groundDrag : 0;
    }
    public void CheckSlope()
    {
        onSlope = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, playerHeight / 2 + 0.25f) 
               && Vector3.Angle(hit.normal, Vector3.up) <= 45f && Vector3.Angle(hit.normal, Vector3.up) > 0;
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

    private void UpdateStandingState()
    {
        transform.localScale = originalScale;
        transform.rotation = Quaternion.identity;
        isCrouching = false;
        isLying = false;
    }
    private void UpdateLyingState()
    {
        Debug.Log(currentStance);
        transform.localScale = originalScale;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(90, 0, 0), transitionSpeed * Time.deltaTime);
        playerModel.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(90, 0, 0), transitionSpeed * Time.deltaTime);
        isCrouching = false;
        isLying = true;
    }
    private void UpdateCrouchState()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(originalScale.x, originalScale.y * 0.5f, originalScale.z), transitionSpeed * Time.deltaTime);
        transform.rotation = Quaternion.identity;
        isCrouching = true;
        isLying = false;
    }

    private void HandleInput()
    {
        horInput = Input.GetAxisRaw("Horizontal");
        verInput = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Crouch"))
            currentStance = currentStance == Stance.Crouching ? Stance.Standing : Stance.Crouching;

        if (Input.GetButtonDown("Prone"))
            currentStance = currentStance == Stance.Lying ? Stance.Standing : Stance.Lying;

        if (Input.GetButtonDown("Faster"))
            speedIndicator++;
        else if (Input.GetButtonDown("Slower"))
            speedIndicator--;

        speedIndicator = Mathf.Clamp(speedIndicator, 0, 3);

        if (Input.GetButton("Jump") && readyToJump)
        {
            currentStance = Stance.Standing;
            Debug.Log("Jumping");
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    private void MovePlayer()
    {
        moveDirection = playerRotation.forward * verInput + playerRotation.right * horInput;
        if (horInput != 0 || verInput != 0)
        {
            float MoveSpeed = GetMoveSpeed(currentStance);
            if (isGrounded)
                rb.AddForce(MoveSpeed * moveDirection.normalized, ForceMode.Force);
            else if (onSlope)
            {
                rb.AddForce(2.5f * MoveSpeed * moveDirection.normalized, ForceMode.Force);
                rb.drag *= 2;
            }
            else if (!isGrounded && !onSlope)
                rb.AddForce(5f * airMultiplier * MoveSpeed * moveDirection.normalized, ForceMode.Force);
        }
        else
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);
        }
    }
    private float GetMoveSpeed(Stance stance)
    {
        return moveSpeeds[stance][Mathf.Clamp(speedIndicator, 0, moveSpeeds[stance].Length - 1)];
    }
    private void Jump()
    {
        if (isGrounded || onSlope)
        {
            rb.velocity = new(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
}
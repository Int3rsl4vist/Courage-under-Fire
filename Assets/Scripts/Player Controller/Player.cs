using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Cam Settings")]
    [SerializeField] float mouseSens = 3f;
    [SerializeField] public Transform camTransform;
    
    [Header("Move Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float climbingSpeed = 2.5f;
    [SerializeField] float mass = 1.5f;
    [SerializeField] float acceleration = 20f;
    [SerializeField] float BottomWorldBoundary = -30f;

    public bool IsGrounded => controller.isGrounded;

    public float Height
    {
        get => controller.height;
        set => controller.height = value;
    }

    public event Action OnBeforeMove;
    public event Action<bool> OnGroundStateChange;

    internal float speedMultiplier;

    private State _state;

    public State CurrentState
    {
        get => _state;
        set
        {
            _state = value;
            velocity = Vector3.zero;
        }
    }
    public enum State
    {
        Walking, Climbing
    }

    CharacterController controller;
    internal Vector3 velocity;
    Vector2 lookDirection;

    (Vector3, Quaternion) curPosAndRot;

    bool wasGrounded;

    PlayerInput controls;
    InputAction moveAction;
    InputAction lookAction;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        controls = GetComponent<PlayerInput>();
        moveAction = controls.actions["Move"];
        lookAction = controls.actions["Look"];
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        curPosAndRot = (transform.position, transform.rotation);
    }

    void Teleport(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        Physics.SyncTransforms();
        lookDirection.x = rotation.eulerAngles.y;
        lookDirection.y = rotation.eulerAngles.z;
        velocity = Vector3.zero;
    }
    
    void Update()
    {
        speedMultiplier = 1f;
        switch (CurrentState)
        {
            case State.Walking:
                UpdateGround();
                UpdateGravity();
                UpdateMovement();
                UpdateLook();
                CheckBounds();
                break;
            case State.Climbing:
                UpdateMovementClimbing();
                UpdateLook();
                break;
        }
    }

    private void CheckBounds()
    {
        if(transform.position.y < BottomWorldBoundary)
        {
            var(position, rotation) = curPosAndRot;
            Teleport(position, rotation);
        }
    }

    private void UpdateGround()
    {
        if(wasGrounded != IsGrounded)
        {
            OnGroundStateChange?.Invoke(IsGrounded);
            wasGrounded = IsGrounded;
        }
    }

    private void UpdateGravity()
    {
        var gravity = mass * Time.deltaTime * Physics.gravity;
        velocity.y = controller.isGrounded ? -1f : velocity.y + gravity.y;
    }

    private void UpdateMovement()
    {
        OnBeforeMove?.Invoke();
        Vector3 moveInput = GetMovementInput(moveSpeed);

        float accelerationFactor = acceleration * Time.deltaTime;
        velocity.x = Mathf.Lerp(velocity.x, moveInput.x, accelerationFactor);
        velocity.z = Mathf.Lerp(velocity.z, moveInput.z, accelerationFactor);

        controller.Move(velocity * Time.deltaTime);
    }

    Vector3 GetMovementInput(float speed, bool horizontal = true)
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 input = new();
        var referenceTransform = horizontal ? transform : camTransform;
        input += referenceTransform.forward * moveInput.y;
        input += referenceTransform.right * moveInput.x;
        input = Vector3.ClampMagnitude(input, 1f);
        input *= speed * speedMultiplier;
        return input;
    }

    void UpdateMovementClimbing()
    {
        Vector3 input = GetMovementInput(climbingSpeed, false);
        float inputForwardFactor = Vector3.Dot(transform.forward, input.normalized);

        if(inputForwardFactor > 0f)
        {
            input.x *= .5f;
            input.z *= .5f;

            if (Mathf.Abs(input.y) > 0.2f)
                input.y = Mathf.Sign(input.y) * climbingSpeed;
        }
        else
        {
            input.x *= 3f;
            input.y = 0;
            input.z *= 3f;
        }
        input.x *= .5f;
        input.z *= .5f;

        if (Mathf.Abs(input.y) > 0.2f)
            input.y = Mathf.Sign(input.y) * climbingSpeed;

        float accelerationFactor = acceleration * Time.deltaTime;
        velocity = Vector3.Lerp(velocity, input, accelerationFactor);

        controller.Move(velocity * Time.deltaTime);
    }
    void UpdateLook()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        lookDirection.x += lookInput.x * mouseSens;
        lookDirection.y += lookInput.y * mouseSens;

        lookDirection.y = Mathf.Clamp(lookDirection.y, -90, 90);

        camTransform.localRotation = Quaternion.Euler(-lookDirection.y, 0, 0);
        transform.localRotation = Quaternion.Euler(0, lookDirection.x, 0);
    }
}
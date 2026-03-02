using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Tohle ti zachrání prdel, když omylem smažeš komponenty v Inspectoru
[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    [Header("Cam Settings")]
    [SerializeField] float mouseSens = 3f;
    public Transform camTransform;

    [Header("Move Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float climbingSpeed = 2.5f;
    [SerializeField] float mass = 1.5f;
    [SerializeField] float acceleration = 20f;
    [SerializeField] float bottomWorldBoundary = -30f;

    public bool IsGrounded => controller.isGrounded;

    public float Height
    {
        get => controller.height;
        set => controller.height = value;
    }

    public event Action OnBeforeMove;
    public event Action<bool> OnGroundStateChange;

    internal float speedMultiplier = 1f;
    internal Vector3 velocity;

    public enum State { Walking, Climbing }
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

    private CharacterController controller;
    private PlayerInput controls;
    private InputAction moveAction;
    private InputAction lookAction;

    private Vector2 lookDirection;
    private (Vector3 pos, Quaternion rot) startPosAndRot;
    private bool wasGrounded;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        controls = GetComponent<PlayerInput>();

        moveAction = controls.actions["Move"];
        lookAction = controls.actions["Look"];
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        startPosAndRot = (transform.position, transform.rotation);
        lookDirection.x = transform.eulerAngles.y;
    }

    private void Update()
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
        if (transform.position.y < bottomWorldBoundary)
        {
            Teleport(startPosAndRot.pos, startPosAndRot.rot);
        }
    }

    private void Teleport(Vector3 position, Quaternion rotation)
    {
        controller.enabled = false;
        transform.SetPositionAndRotation(position, rotation);
        controller.enabled = true;

        lookDirection.x = rotation.eulerAngles.y;
        lookDirection.y = rotation.eulerAngles.z;
        velocity = Vector3.zero;
    }

    private void UpdateGround()
    {
        if (wasGrounded != IsGrounded)
        {
            OnGroundStateChange?.Invoke(IsGrounded);
            wasGrounded = IsGrounded;
        }
    }

    private void UpdateGravity()
    {
        var gravity = mass * Time.deltaTime * Physics.gravity;
        velocity.y = IsGrounded ? -1f : velocity.y + gravity.y;
    }

    private void UpdateMovement()
    {
        OnBeforeMove?.Invoke();

        Vector3 moveInput = GetMovementInput(moveSpeed, true);
        float accelerationFactor = acceleration * Time.deltaTime;

        velocity.x = Mathf.Lerp(velocity.x, moveInput.x, accelerationFactor);
        velocity.z = Mathf.Lerp(velocity.z, moveInput.z, accelerationFactor);

        controller.Move(velocity * Time.deltaTime);
    }

    private Vector3 GetMovementInput(float speed, bool horizontal)
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Transform referenceTransform = horizontal ? transform : camTransform;

        Vector3 input = (referenceTransform.forward * moveInput.y) + (referenceTransform.right * moveInput.x);
        input = Vector3.ClampMagnitude(input, 1f);

        return input * (speed * speedMultiplier);
    }
    private void UpdateMovementClimbing()
    {
        Vector3 input = GetMovementInput(climbingSpeed, false);
        float inputForwardFactor = Vector3.Dot(transform.forward, input.normalized);

        if (inputForwardFactor > 0f)
        {
            input.x *= 0.5f;
            input.z *= 0.5f;
            if (Mathf.Abs(input.y) > 0.2f) input.y = Mathf.Sign(input.y) * climbingSpeed;
        }
        else
        {
            input.x *= 3f;
            input.y = 0f;
            input.z *= 3f;
        }

        input.x *= 0.5f;
        input.z *= 0.5f;

        if (Mathf.Abs(input.y) > 0.2f) input.y = Mathf.Sign(input.y) * climbingSpeed;

        float accelerationFactor = acceleration * Time.deltaTime;
        velocity = Vector3.Lerp(velocity, input, accelerationFactor);

        controller.Move(velocity * Time.deltaTime);
    }
    private void UpdateLook()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        lookDirection.x += lookInput.x * mouseSens;
        lookDirection.y += lookInput.y * mouseSens;
        lookDirection.y = Mathf.Clamp(lookDirection.y, -90f, 90f);
        camTransform.localRotation = Quaternion.Euler(-lookDirection.y, 0, 0);
        transform.localRotation = Quaternion.Euler(0, lookDirection.x, 0);
    }
}
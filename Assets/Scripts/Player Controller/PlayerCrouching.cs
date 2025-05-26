using Codice.CM.Common.Tree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCrouching : MonoBehaviour
{
    [SerializeField] float crouchHeight = 1f;
    [SerializeField] float transitionSpeed = 2.5f;
    [SerializeField] float crouchSpeedMultiplier = 0.5f;
    Player player;
    PlayerInput playerInput;
    InputAction crouchAction;
    public Transform playerModel;

    Vector3 origCamPos;

    float curHeight;
    float standingHeight;

    bool IsCrouching => standingHeight - curHeight > 0.1f;

    void Awake()
    {
        player = GetComponent<Player>();
        playerInput = GetComponent<PlayerInput>();
        crouchAction = playerInput.actions["Crouch"];
    }
    void Start()
    {
        origCamPos = player.camTransform.localPosition;
        standingHeight = curHeight = player.Height;
        crouchHeight = player.Height / 3;
    }
    void OnEnable() => player.OnBeforeMove += OnBeforeMove;
    void OnDisable() => player.OnBeforeMove -= OnBeforeMove;

    void OnBeforeMove()
    {
        bool crouchAttempted = crouchAction.ReadValue<float>() > 0;

        float targetHeight = crouchAttempted ? crouchHeight : standingHeight;

        if (IsCrouching && !crouchAttempted)
        {
            Vector3 raySpawner = transform.position + new Vector3(0, curHeight / 2, 0);
            if (Physics.Raycast(raySpawner, Vector3.up, out RaycastHit hit, 0.2f))
            {
                float topSpace = hit.point.y - raySpawner.y;
                targetHeight = Mathf.Max(curHeight + topSpace - 0.1f, crouchHeight);
            }
        }
        if(!Mathf.Approximately(targetHeight, curHeight))
        {
            float crouchDiff = Time.deltaTime * transitionSpeed;
            curHeight = Mathf.Lerp(curHeight, targetHeight, crouchDiff);

            Vector3 heightDiff = new(0, (standingHeight - curHeight) / 2, 0);
            Vector3 newCamPos = origCamPos - heightDiff;

            player.camTransform.localPosition = newCamPos;
            player.Height = curHeight;
        }
        if(IsCrouching)
        {
            player.speedMultiplier *= crouchSpeedMultiplier;
        }
    }
}

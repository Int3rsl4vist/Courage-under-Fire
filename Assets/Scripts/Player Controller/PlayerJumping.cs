using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerJumping : MonoBehaviour
{
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float jumpBufferTime = 0.05f;
    [SerializeField] float groundContactTime = 0.2f;

    Player player;

    bool tryingToJump;
    float lastJumpTime;
    float lastGroundedTime;

    private void Awake()
    {
        player = GetComponent<Player>();
    }
    private void OnEnable()
    {
        player.OnBeforeMove += OnBeforeMove;
        player.OnGroundStateChange += OnGroundStateChange;
    }
    private void OnDisable()
    {
        player.OnBeforeMove -= OnBeforeMove;
        player.OnGroundStateChange -= OnGroundStateChange;
    }
    void OnJump()
    {
        tryingToJump = true;
        lastJumpTime = Time.time;
    }

    void OnBeforeMove()
    {
        bool triedToJump = Time.time - lastJumpTime < jumpBufferTime;
        bool wasGrounded = Time.time - lastGroundedTime < groundContactTime;
        bool jumpAttempted = tryingToJump || (triedToJump && player.IsGrounded);
        bool groundContact = player.IsGrounded || wasGrounded;
        if (jumpAttempted && groundContact)
            player.velocity.y += jumpSpeed;
        tryingToJump = false;
    }

    void OnGroundStateChange(bool isGrounded)
    {
        if(!isGrounded)
            lastGroundedTime = Time.time;
    }
}

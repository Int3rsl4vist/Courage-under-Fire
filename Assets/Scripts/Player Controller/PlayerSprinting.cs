using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerSprinting : MonoBehaviour
{
    [SerializeField] float speedMultiplier = 2f;
    Player player;
    PlayerInput playerInput;
    InputAction sprintAction;

    private void Awake()
    {
        player = GetComponent<Player>();
        playerInput = GetComponent<PlayerInput>();
        sprintAction = playerInput.actions["Sprint"];
    }
    private void OnEnable() => player.OnBeforeMove += OnBeforeMove;
    private void OnDisable() => player.OnBeforeMove -= OnBeforeMove;
    void OnBeforeMove()
    {
        float sprintInput = sprintAction.ReadValue<float>();
        player.speedMultiplier *= sprintInput > 0 ? speedMultiplier : 1f;
    }
}

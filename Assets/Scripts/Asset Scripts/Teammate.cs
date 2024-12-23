using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teammate : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float minDistFromPlayer = 2;
    private Transform player;
    public bool isNextToPlayer = false;
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;


    public enum CrewCommands { Follow, Halt}
    public CrewCommands currentCommand;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        currentHealth = maxHealth;
        currentCommand = CrewCommands.Follow;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentCommand)
        {
            case CrewCommands.Follow:
                MoveTowards(player, minDistFromPlayer);
                break;
            case CrewCommands.Halt:
                transform.Translate(Vector3.zero);
                break;
            default:
                break;
        }
        
    }

    private void MoveTowards(Transform reference, float stoppingDistance)
    {
        if (player != null)
        {
            Vector3 direction = (reference.position - transform.position).normalized;

            float distanceFromObject = Vector3.Distance(transform.position, player.position);
            Debug.Log($"Distance from player:{distanceFromObject}");
            if (distanceFromObject > stoppingDistance)
                transform.Translate(moveSpeed * Time.deltaTime * direction);
            else if(distanceFromObject == stoppingDistance)
                transform.Translate(Vector3.zero);
            else
            {
                Vector3 retreatDirection = (transform.position - reference.position).normalized;
                transform.Translate(moveSpeed * Time.deltaTime * retreatDirection);
            }
        }
    }
}

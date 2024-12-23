using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float stoppingDistance = 2;
    private Transform player;
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    private void Move()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;

            float distanceFromPlayer = Vector3.Distance(transform.position, player.position);
            Debug.Log($"Distance from player:{distanceFromPlayer}");
            if (distanceFromPlayer > stoppingDistance)
            {
                transform.Translate(moveSpeed * Time.deltaTime * direction);
            }
            else
            {
                Vector3 retreatDirection = (transform.position - player.position).normalized;
                transform.Translate(moveSpeed * Time.deltaTime * retreatDirection);
            }
        }
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        Destroy(gameObject);
    }
}

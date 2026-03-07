using Unity.VisualScripting;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    [Tooltip("How much damage the Player takes")]
    public float damageAmount = 15f;

    [Tooltip("How often the Player takes damage (seconds)")]
    public float damageInterval = 1f;

    private float _nextDamageTime = 0;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(Time.time >= _nextDamageTime)
            {
                PlayerStats stats = other.GetComponent<PlayerStats>();

                if(stats != null)
                {
                    stats.TakeDamage(damageAmount);
                    _nextDamageTime = Time.time + damageInterval;
                    Debug.Log("CODE_LOG: Player is taking damage");
                }
            }
        }
    }
}

using System;
using UnityEngine;

public class EnemyVision_FailWhenSeen : MonoBehaviour
{
    [Header("Settings:")]
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 90f;

    [Header("Layers:")]
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [Header("References:")]
    public Transform player;
    public Transform eyes;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError($"CODE_ERROR: Player reference missing"); 
            return;
        }
        if (eyes == null)
        {
            Debug.LogError($"CODE_ERROR: Eyes reference missing");
            return;
        }
    }

    private void Update()
    {
        FindVisibleTarget();
    }

    void FindVisibleTarget()
    {
        // 1. VZDÁLENOST
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // DEBUG: Kreslíme èáru k hráèi VŽDY (Èervená = smìr k hráèi)
        Debug.DrawLine(eyes.position, player.position, Color.red);

        // DEBUG: Kreslíme, kam se NPC dívá (Modrá = "Pøedek" NPCèka)
        // Kreslíme to 2 metry dlouhé, aby to bylo vidìt
        Debug.DrawRay(eyes.position, transform.forward * 2f, Color.blue);

        if (distanceToPlayer < viewRadius)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;

            // Plochá matematika (ignorujeme Y)
            Vector3 flatForward = transform.forward; flatForward.y = 0;
            Vector3 flatDir = dirToPlayer; flatDir.y = 0;

            float angle = Vector3.Angle(flatForward, flatDir);

            // Tady si vypíšeme ten úhel, a vidíme, co se dìje
            // Pokud je úhel tøeba 180, znamená to, že NPC kouká dozadu!
            // Debug.Log("Aktuální úhel: " + angle); 

            if (angle < viewAngle / 2)
            {
                // Tady byla ta zelená èára, ke které ses nedostal...
                Vector3 startPos = eyes.position;
                Vector3 targetPos = player.position + Vector3.up * 1.5f;

                // ZELENÁ = Vidím tì v úhlu, zkouším Raycast
                Debug.DrawLine(startPos, targetPos, Color.green);

                RaycastHit hit;
                if (!Physics.Linecast(startPos, targetPos, out hit, obstacleMask))
                {
                    Debug.Log("MÁM TÌ!");
                    CatchPlayer();
                }
            }
        }
    }

    private void CatchPlayer()
    {
        MissionManager.Instance?.FailMission("You were seen!");
        this.enabled = false;
    }

    private void OnDrawGizmos()
    {
        if (eyes == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(eyes.position, viewRadius);

        // Vykreslení úhlu pohledu (zjednodušenì)
        Vector3 leftDir = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;

        Gizmos.DrawLine(eyes.position, eyes.position + leftDir * viewRadius);
        Gizmos.DrawLine(eyes.position, eyes.position + rightDir * viewRadius);
    }
}

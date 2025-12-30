using UnityEngine;

public class MissionObjective : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MissionManager.Instance.CompleteMission();
            //Destroy(gameObject);
        }
    }
}

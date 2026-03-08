using UnityEngine;

public class StageTrigger : MonoBehaviour
{
    public DrillSergeantBrain sergeant;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sergeant.PlayerArrivedAtTrigger();
            gameObject.SetActive(false);
        }
    }
}

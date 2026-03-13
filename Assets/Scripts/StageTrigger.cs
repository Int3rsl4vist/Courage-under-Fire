using Unity.VisualScripting;
using UnityEngine;

public class StageTrigger : MonoBehaviour
{
    public DrillSergeantBrain sergeant;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player detected near {gameObject.name}");
            sergeant.PlayerArrivedAtTrigger();
            Debug.Log($"CODE_LOG: Is DS null:{sergeant == null}; DSBrain alerted");
            gameObject.SetActive(false);
        }
    }
}

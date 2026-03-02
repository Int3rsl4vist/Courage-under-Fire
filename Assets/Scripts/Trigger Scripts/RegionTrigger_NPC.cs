using UnityEngine;

public class RegionTrigger_NPC : MonoBehaviour
{
    [Tooltip("Drag the NPC with the AIMovement script here")]
    public AIMovement npcScript;
    [Tooltip("Only trigger if this tag enters (e.g., 'Player')")]
    public string targetTag = "Player";

    private bool _wasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_wasTriggered)
            return;
        if (other.CompareTag(targetTag))
        {
            if (npcScript != null)
            {
                Debug.Log("CODE_LOG: Player has entered a Trigger Region, signalling NPC");
                npcScript.MoveToNextWaypoint();
                _wasTriggered = true;
            }
            else
                Debug.LogError("CODE_ERROR: NPC Script not assigned to the Trigger Region");
        }
    }
}

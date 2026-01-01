using UnityEngine;

public class MissionObjective : MonoBehaviour
{
    [Header("Mission Settings")]
    public string stepNameID;
    public bool destroyOnTrigger = true;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MissionManager.Instance.CompleteStep(stepNameID);
            if (destroyOnTrigger)
                Destroy(gameObject);
            else
                GetComponent<Collider>().enabled = false;
        }
    }
}

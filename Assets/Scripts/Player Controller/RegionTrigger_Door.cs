using UnityEngine;

public class RegionTrigger_Door : MonoBehaviour
{
    public ObjectController targetObject;
    public bool triggerOnlyOnce = false;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("CODE_LOG: Region Triggered by: " +  other.name);
        if (other.CompareTag("Player") && targetObject != null)
        {
            Debug.Log("CODE_LOG: Player detected - closing Door");
            targetObject.Close();
            if (triggerOnlyOnce)
                GetComponent<Collider>().enabled = false;
        }
        else
            Debug.LogWarning("CODE_WARNING: Fault. Player tag: " + other.tag + ", TargetObject set to: " + (targetObject != null));
    }
}

using UnityEngine;

public class MissionTrigger : MonoBehaviour
{
    [Header("Mission Settings:")]
    public string missionStepID;
    public bool destroyOnTrigger = true;

    private MissionTriggerGroup myGroup;
    private bool alreadyTriggered = false;

    public void RegisterGroup(MissionTriggerGroup group)
    {
        myGroup = group;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (alreadyTriggered) return;
        if (other.CompareTag("Player"))
        {
            alreadyTriggered = true;

            if (myGroup != null)
                myGroup.ReportTriggerHit(this);
            else
                MissionManager.Instance.CompleteStep(missionStepID);
            if (destroyOnTrigger)
                Destroy(gameObject);
            else
                GetComponent<Collider>().enabled = false;
        }
    }
}

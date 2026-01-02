using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Build;

public class MissionTriggerGroup : MonoBehaviour
{
    [Header("Settings:")]
    public string stepNameID;
    public bool requireAll = false;

    private List<MissionTrigger> myTriggers = new();
    private int triggersHitCount = 0;
    private bool groupFinished = false;

    private void Start()
    {
        MissionTrigger[] foundTriggers = GetComponentsInChildren<MissionTrigger>();

        foreach (var t in foundTriggers)
        {
            myTriggers.Add(t);
            t.RegisterGroup(this);
        }
    }

    public void ReportTriggerHit(MissionTrigger t)
    {
        if (groupFinished) return;

        triggersHitCount++;

        if (requireAll)
        {
            Debug.Log($"CODE_LOG: Checkpoint complete: {triggersHitCount} / {myTriggers.Count}");

            if (triggersHitCount >= myTriggers.Count)
                CompleteObjective();
        }
        else
            CompleteObjective();
    }
    void CompleteObjective()
    {
        groupFinished = true;

        Debug.Log("CODE_LOG: Trigger group complete -> engaging the Manager");

        MissionManager.Instance.CompleteStep(stepNameID);
    }
}

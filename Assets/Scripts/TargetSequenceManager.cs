using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TargetSequenceManager : MonoBehaviour
{
    [Header("Mission Link:")]
    public string missionStepID;

    [Header("Targets:")]
    public List<ShootingTarget> targets;

    [Header("Settings:")]
    public float targetDelay = 1.5f;

    [Header("Drill Sergeant Link:")]
    public DrillSergeantBrain sergeant;
    public UnityEvent onSequenceComplete;

    private int _curTargetIndex = 0;

    private void Start()
    {
        foreach(var target in targets)
        {
            if(target != null)
                target.Setup(this);
        }
    }
    public void StartSequence()
    {
        _curTargetIndex = 0;
        if (targets.Count > 0)
            StartCoroutine(DelayFirstTarget());
    }
    IEnumerator DelayFirstTarget()
    {
        yield return new WaitForSeconds(1f);
        targets[0].PopUp();
    }
    public void TargetHit(ShootingTarget hitTarget)
    {
        if (hitTarget != targets[_curTargetIndex]) return;

        _curTargetIndex++;

        if(_curTargetIndex < targets.Count)
        {
            StartCoroutine(RaiseNextTarget_Delayed());
        }
        else
        {
            Debug.Log("CODE_LOG: ShootingRange cleared");
            if(MissionManager.Instance != null && !string.IsNullOrEmpty(missionStepID))
                MissionManager.Instance.CompleteStep(missionStepID);
            if(sergeant != null)
                sergeant.TargetsCleared();
            onSequenceComplete?.Invoke();
        }
    }
    IEnumerator RaiseNextTarget_Delayed()
    {
        yield return new WaitForSeconds(targetDelay);
        targets[_curTargetIndex].PopUp();
    }
}

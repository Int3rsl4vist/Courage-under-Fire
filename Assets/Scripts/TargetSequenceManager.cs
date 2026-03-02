using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSequenceManager : MonoBehaviour
{
    [Header("Mision Link:")]
    public string missionStepID;

    [Header("Targets:")]
    public List<ShootingTarget> targets;

    [Header("Settings:")]
    public float targetDelay = 1.5f;

    private int _curTargetIndex = 0;

    private void Start()
    {
        foreach(var target in targets)
        {
            target.Setup(this);
        }
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
            MissionManager.Instance?.CompleteStep(missionStepID);
        }
    }
    IEnumerator RaiseNextTarget_Delayed()
    {
        yield return new WaitForSeconds(targetDelay);

        targets[_curTargetIndex].PopUp();
    }
}

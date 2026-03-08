using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[System.Serializable]
public class BootcampStage
{
    public string stageName = "ShootingRange_1";
    public Transform sergeantDestiantion;

    [Header("Events (insert dialogues):")]
    public UnityEvent onPlayerArrived;
    public UnityEvent onTargetsCleared;
}
[RequireComponent(typeof(NavMeshAgent))]
public class DrillSergeantBrain : MonoBehaviour
{
    [Header("Stages:")]
    public BootcampStage[] stages;
    public int currentStageIndex = 0;
    
    public enum State { Idle, MovingToPos, WaitingForPlayer, WaitingForTargets}
    public State currentState = State.Idle;

    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        if (currentState == State.MovingToPos)
        {
            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                if(!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                {
                    currentState = State.WaitingForPlayer;
                    Debug.Log($"CODE_LOG: Drill Sergeant arrived at position '{stages[currentStageIndex].stageName}' and is waiting on the Player");
                }
            }
        }
    }
    public void StartMovingToNextStage()
    {
        if(currentStageIndex >= stages.Length)
        {
            Debug.Log("CODE_LOG: Sequence done, returning to starting position");
            return;
        }

        _agent.SetDestination(stages[currentStageIndex].sergeantDestiantion.position);
        currentState = State.MovingToPos;
        Debug.Log($"CODE_LOG: Drill Sergeant is moving to stage {stages[currentStageIndex].stageName}");
    }
    public void PlayerArrivedAtTrigger()
    {
        if (currentState != State.WaitingForPlayer) return;

        currentState = State.WaitingForTargets;
        stages[currentStageIndex].onPlayerArrived?.Invoke();
    }
    public void TargetsCleared()
    {
        if(currentState != State.WaitingForTargets) return;
        
        stages[currentStageIndex].onTargetsCleared?.Invoke();
        currentStageIndex++;
        currentState = State.Idle;
    }
}
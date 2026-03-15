using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[System.Serializable]
public class BootcampStage
{
    public string stageName = "ShootingRange_1";
    public Transform sergeantDestiantion;

    [Tooltip("Should the NPC wait for the player after arriving at its destination? If NOT, the NPC will start its Dialogue Sequence right after arrival")]
    public bool waitForPlayerTrigger = true; 

    [Header("Events (insert dialogues):")]
    public UnityEvent onPlayerArrived;
    public UnityEvent onTargetsCleared;
    public UnityEvent onNPCArrived;
}
[RequireComponent(typeof(NavMeshAgent))]
public class DrillSergeantBrain : MonoBehaviour
{
    [Header("Stages:")]
    public BootcampStage[] stages;
    public int currentStageIndex = 0;
    
    public enum State { Idle, MovingToPos, WaitingForPlayer, WaitingForTargets}
    public State currentState = State.Idle;

    [Header("Rotation Setup:")]
    public float turnSpeed = 270f;

    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        Debug.Log($"DrillSergeantBrain is active");
    }
    private void Update()
    {
        if (currentState == State.MovingToPos)
        {
            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                {
                    if (stages[currentStageIndex].waitForPlayerTrigger)
                    {
                        currentState = State.WaitingForPlayer;
                        stages[currentStageIndex].onNPCArrived.Invoke();
                        Debug.Log($"CODE_LOG: Drill Sergeant arrived at position '{stages[currentStageIndex].stageName}' and is waiting on the Player");
                    }
                    else
                    {
                        currentState = State.WaitingForTargets;
                        Debug.Log($"CODE_LOG: Drill Sergeant is at position '{stages[currentStageIndex].stageName}' and is commencing dialogues");
                        stages[currentStageIndex].onPlayerArrived?.Invoke();
                    }
                }
            }
        }
        else if (currentState == State.WaitingForPlayer || currentState == State.WaitingForTargets)
        {
            AlignWithTargetRotation();
        }
    }

    private void AlignWithTargetRotation()
    {
        if (currentStageIndex >= stages.Length) return;

        Transform targetTransform = stages[currentStageIndex].sergeantDestiantion;
        if(targetTransform != null)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetTransform.rotation, Time.deltaTime * turnSpeed);
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
        Debug.Log($"CODE_LOG: Missionanager reports all targets cleared. Current state: '{currentState}'");
        if(currentState != State.WaitingForTargets)
        {
            Debug.Log("CODE_LOG: No targets to wait for");
            return;
        }

        Debug.Log("CODE_LOG: Current state is WaitingForTargets, triggering event and switching state to Idle");
        stages[currentStageIndex].onTargetsCleared?.Invoke();
        currentStageIndex++;
        currentState = State.Idle;
    }
}
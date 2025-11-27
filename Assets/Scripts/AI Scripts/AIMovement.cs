using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    [Tooltip("Drag the destination object here")]
    public Transform targetDestination;

    private bool _hasArrived = false;
    private bool _hasStarted = false;

    private void Start()
    {
        if(agent == null)
            agent = GetComponent<NavMeshAgent>();
        if(targetDestination == null)
        {
            Debug.LogError("CODE_ERROR: No Target Destination assigned in Inspector");
            return;
        }
    }
    private void Update()
    {
        if (_hasStarted && !_hasArrived)
        {
            if(!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                if(!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    _hasArrived = true;
                    Debug.Log("NPC has reached the destination");
                }
            }
        }
    }
    public void SetDestinationToTarget()
    {
        if (targetDestination == null)
            return;
        if (NavMesh.SamplePosition(targetDestination.position, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
        {
            NavMeshPath path = new();
            agent.CalculatePath(hit.position, path);
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                agent.SetDestination(hit.position);
                _hasStarted = true;
                Debug.Log("CODE_LOG: Target reachable, initiating movement");
            }
            else if (path.status == NavMeshPathStatus.PathPartial)
            {
                agent.SetDestination(hit.position);
                _hasStarted = true;
                Debug.LogWarning("CODE_WARNING: Target is unreachable. Something is blocking the path. Moving as close as possible");
            }
            else
                Debug.LogError("CODE_ERROR: Target is invalid");
        }
        else
            Debug.LogError("CODE_ERROR: The Target Destination is not on a valid NavMesh");
    }
}
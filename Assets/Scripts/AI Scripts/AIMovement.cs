using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;

[RequireComponent(typeof(NavMeshAgent))]
public class AIMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    [Tooltip("Drag all the destination objects here")]
    public Transform[] waypoints;

    private int _currentWaypointIndex = -1;
    private bool _isMoving = false;

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
        if (waypoints == null || waypoints.Length == 0)
            Debug.LogError($"CODE_ERROR: No waypoints assigned. NPC '{gameObject.name}' has nowhere to go");
    }
    private void Update()
    {
        if (_isMoving)
        {
            if(!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                if(!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    _isMoving = false;
                    Debug.Log($"CODE_LOG: NPC '{gameObject.name}' reached waypoint {_currentWaypointIndex} and awaits next waypoint");
                }
            }

        }
    }
    public void MoveToNextWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        _currentWaypointIndex++;

        if(_currentWaypointIndex >= waypoints.Length)
        {
            Debug.LogWarning($"CODE_WARNING: NPC '{gameObject.name}' reached its destination. It has nowhere to go");
            return;
        }
        Transform target = waypoints[_currentWaypointIndex];

        if (target == null)
        {
            Debug.LogError($"CODE_ERROR: Waypoint {_currentWaypointIndex} is null. Trying the next one");
            MoveToNextWaypoint();
            return;
        }
        SetDestination(target.position);
    }
    private void SetDestination(Vector3 position)
    {
        if(NavMesh.SamplePosition(position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            NavMeshPath path = new();
            agent.CalculatePath(hit.position, path);

            if(path.status == NavMeshPathStatus.PathComplete)
            {
                agent.SetDestination(hit.position);
                _isMoving = true;
                Debug.Log($"CODE_LOG: NPC '{gameObject.name}' is moving to waypoint {_currentWaypointIndex}");
            }
            else if (path.status == NavMeshPathStatus.PathPartial)
            {
                agent.SetDestination(hit.position);
                _isMoving = true;
                Debug.LogWarning($"CODE_WARNING: NPC '{gameObject.name}' may not reach waypoint {_currentWaypointIndex} (the path is blocked). Initiating movement");
            }
            else
            {
                Debug.LogError($"CODE_ERROR: No path found to waypoint {_currentWaypointIndex} found");
            }
        }
        else
        {
            Debug.LogError($"CODE_ERROR: Waypoint {_currentWaypointIndex} is not on the NavMesh");
        }
    }
}
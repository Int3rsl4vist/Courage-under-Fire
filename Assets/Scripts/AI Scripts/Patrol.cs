using UnityEngine;
using UnityEngine.AI;

public class Patrol : MonoBehaviour
{
    [Header("Patrol route stup")]
    public Transform[] waypoints;

    private NavMeshAgent _agent;
    private int _currentWaypointIndex = 0;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        if(waypoints.Length == 0)
        {
            Debug.LogError("CODE_ERROR: No waypoints set");
            enabled = false;
            return;
        }
        GoToNextPoint();
    }
    private void Update()
    {
        if (!_agent.pathPending && _agent.remainingDistance < _agent.stoppingDistance + .5f)
            GoToNextPoint();
    }

    private void GoToNextPoint()
    {
        if(waypoints.Length == 0)
            return;
        _agent.SetDestination(waypoints[_currentWaypointIndex].position);
        _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Length;
    }
}

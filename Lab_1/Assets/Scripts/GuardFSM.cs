using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardFSM : MonoBehaviour
{
    NavMeshAgent agent;

    [SerializeField] private Transform player;

    [SerializeField] private List<Transform> waypoints = new List<Transform>(4);
    int currentWaypoint = 0;


    enum GuardState
    {
        Patrolling,
        Chasing,
        Returning
    }

    GuardState state = GuardState.Patrolling;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

        switch (state)
        {
            case GuardState.Patrolling:
                if(!agent.pathPending && agent.remainingDistance < 1f) {
                    agent.SetDestination(waypoints[currentWaypoint].position);
                    currentWaypoint = (currentWaypoint + 1) % waypoints.Count;
                }
                break;
            case GuardState.Chasing:
                agent.SetDestination(player.position);
                break;
            case GuardState.Returning:
                if (agent.remainingDistance < 1f)
                {
                    Debug.Log("Returned to patrol.");
                    state = GuardState.Patrolling;
                }
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected!");
            state = GuardState.Chasing;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player lost!");
            state = GuardState.Returning;
            agent.SetDestination(waypoints[currentWaypoint].position);
        }
    }
}

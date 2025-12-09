using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardFSM : MonoBehaviour
{
    NavMeshAgent agent;
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
                if(agent.remainingDistance < 1f) {
                    agent.SetDestination(waypoints[currentWaypoint].position);
                    currentWaypoint = (currentWaypoint + 1) % waypoints.Count;
                }
                break;
            case GuardState.Chasing:
                break;
            case GuardState.Returning:
                break;
        }
    }
}

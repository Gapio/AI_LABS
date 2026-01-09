using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BossMovement : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float stopDistance = 2.6f;
    [SerializeField] float repathInterval = 0.15f;

    NavMeshAgent agent;
    float nextRepath;

    float defaultSpeed;
    float defaultAngularSpeed;
    float currentYaw;


    public bool IsDashing { get; private set; }

    float yawVel;

    bool AgentActive => agent != null && agent.enabled && agent.isOnNavMesh;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stopDistance;

        defaultSpeed = agent.speed;
        defaultAngularSpeed = agent.angularSpeed;

        agent.updateRotation = true;

        currentYaw = transform.eulerAngles.y;

    }

    public void SetTarget(Transform t) => target = t;

    public void BeginChaseRotation()
    {
        if (!AgentActive) return;
        agent.updateRotation = true;
        agent.angularSpeed = defaultAngularSpeed;
        currentYaw = transform.eulerAngles.y;
        yawVel = 0f;

    }

    public void BeginAttackRotation()
    {
        if (!AgentActive) return;

        agent.updateRotation = false;

        currentYaw = transform.eulerAngles.y;
        yawVel = 0f;
    }


    public void StartChase()
    {
        if (IsDashing || !AgentActive) return;
        BeginChaseRotation();
        agent.speed = defaultSpeed;
        agent.isStopped = false;
    }

    public void StartSlowFollow(float speedMultiplier = 0.35f)
    {
        if (IsDashing || !AgentActive) return;

        BeginAttackRotation(); // critical: keep agent.updateRotation = false during attack wind-up
        agent.speed = defaultSpeed * Mathf.Clamp01(speedMultiplier);
        agent.isStopped = false;
    }


    public void Stop()
    {
        if (IsDashing || !AgentActive) return;
        agent.isStopped = true;
        agent.ResetPath();
    }

    public void TickChase()
    {
        if (IsDashing || !AgentActive || !target) return;
        if (Time.time < nextRepath) return;

        nextRepath = Time.time + repathInterval;
        agent.SetDestination(target.position);
    }

    public void FaceTargetDamped(float turnSmoothTime = 0.08f)
    {
        if (!target) return;

        Vector3 dir = target.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;

        float targetYaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        currentYaw = Mathf.SmoothDampAngle(currentYaw, targetYaw, ref yawVel, turnSmoothTime);

        transform.rotation = Quaternion.Euler(0f, currentYaw, 0f);
    }


    public void DashAwayFromTarget(float distance, float duration)
    {
        if (!target || IsDashing) return;
        StartCoroutine(DashRoutine(distance, duration));
    }

    IEnumerator DashRoutine(float distance, float duration)
    {
        IsDashing = true;

        if (AgentActive)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        agent.enabled = false;

        Vector3 dir = transform.position - target.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) dir = -transform.forward;
        dir.Normalize();

        Vector3 start = transform.position;
        Vector3 end = start + dir * distance;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, duration);
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        agent.enabled = true;
        if (agent.isOnNavMesh) agent.Warp(transform.position);

        IsDashing = false;
    }
}

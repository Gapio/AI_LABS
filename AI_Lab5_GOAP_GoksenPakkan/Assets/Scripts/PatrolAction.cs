using UnityEngine;
using UnityEngine.UIElements;

public class PatrolAction : GoapActionBase
{
    public float arriveDistance = 0.67f;
    void Reset()
    {
        actionName = "Patrol (One Step)";
        cost = 2f;
        // No planner preconditions; goal selection handles “patrol only when not chasing”.
        preMask = 0;
        // This is intentionally a "one-step completion" fact.
        addMask = GoapBits.Mask(GoapFact.PatrolStepDone);
        delMask = 0;
    }
    public override void OnEnter(GoapContext ctx)
    {
        if (ctx.PatrolWaypoints == null ||
        ctx.PatrolWaypoints.Length == 0) return;
        ctx.Agent.SetDestination(ctx.PatrolWaypoints[ctx.PatrolIndex].position);
    }
    public override GoapStatus Tick(GoapContext ctx)
    {
        // Debug.Log($"PATROL Tick | idx={ctx.PatrolIndex} remDist={ctx.Agent.remainingDistance:0.00} " + $"pending={ctx.Agent.pathPending} status={ctx.Agent.pathStatus}");

        if (ctx.Sensors != null && ctx.Sensors.SeesPlayer)
            return GoapStatus.Failure;

        if (ctx.PatrolWaypoints == null || ctx.PatrolWaypoints.Length == 0)
            return GoapStatus.Failure;

        if (!ctx.Agent.pathPending &&
            ctx.Agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
            return GoapStatus.Failure;

        bool arrived =
            !ctx.Agent.pathPending &&
            ctx.Agent.remainingDistance <= ctx.Agent.stoppingDistance + arriveDistance &&
            ctx.Agent.velocity.sqrMagnitude < 0.01f;

        if (arrived)
        {
            ctx.PatrolIndex = (ctx.PatrolIndex + 1) % ctx.PatrolWaypoints.Length;
            ctx.Agent.SetDestination(ctx.PatrolWaypoints[ctx.PatrolIndex].position);
            return GoapStatus.Success;
        }

        return GoapStatus.Running;
    }
}

using UnityEngine;

public class MoveToPlayerAction : GoapActionBase
{
    public float arriveDistance = 1.2f;
    void Reset()
    {
        actionName = "Move To Player";
        cost = 1f;
        preMask = GoapBits.Mask(GoapFact.SeesPlayer, GoapFact.HasWeapon);
        addMask = GoapBits.Mask(GoapFact.AtPlayer);
        delMask = 0;
    }

    public override bool CheckProcedural(GoapContext ctx)
    {
        return ctx.Player != null;
    }
    public override GoapStatus Tick(GoapContext ctx)
    {
        if (ctx.Player == null) return GoapStatus.Failure;
        if (ctx.Sensors != null && !ctx.Sensors.SeesPlayer) return GoapStatus.Failure;

        ctx.Agent.SetDestination(ctx.Player.position);

        float d = Vector3.Distance(ctx.Agent.transform.position, ctx.Player.position);
        if (d <= arriveDistance) return GoapStatus.Success;

        return GoapStatus.Running;
    }
}

using UnityEngine;

public class TagPlayerAction : GoapActionBase
{
    public float tagDistance = 1.2f;
    void Reset()
    {
        actionName = "Tag Player";
        cost = 1f;
        preMask = GoapBits.Mask(GoapFact.HasWeapon,GoapFact.AtPlayer);
        addMask = GoapBits.Mask(GoapFact.PlayerTagged);
        delMask = GoapBits.Mask(GoapFact.AtPlayer);
    }
    public override GoapStatus Tick(GoapContext ctx)
    {
        if (ctx.Player == null) return GoapStatus.Failure;
        if (ctx.Sensors != null && !ctx.Sensors.SeesPlayer) return GoapStatus.Failure;

        float d = Vector3.Distance(ctx.Agent.transform.position, ctx.Player.position);
        if (d > tagDistance) return GoapStatus.Failure;

        Debug.Log("GOAP: Tagged intruder!");
        return GoapStatus.Success;
    }

}

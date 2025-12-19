using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Update Perception", description: "Updates Target/LOS/LastKnownPosition from GuardSensors", 
    story: "Update perception and write to the blackboard.", category: "Action/Sensing", id: "81bb6fa6b4c9afec2031be95c2afdcd1")]
public class UpdatePerceptionAction : Action
{
    [SerializeReference]
    public BlackboardVariable<GameObject> Self;

    [SerializeReference]
    public BlackboardVariable<GameObject> Target;

    [SerializeReference]
    public BlackboardVariable<bool> HasLineOfSight;

    [SerializeReference]
    public BlackboardVariable<Vector3>

    LastKnownPosition;

    [SerializeReference]
    public BlackboardVariable<float>

    TimeSinceLastSeen;

    protected override Node.Status OnStart()
    {
        if (TimeSinceLastSeen != null && TimeSinceLastSeen.Value < 0f)
            TimeSinceLastSeen.Value = 9999f;
        return Node.Status.Running;
    }
    protected override Node.Status OnUpdate()
    {
        var owner = Self != null ? Self.Value : null;
        var sensors = owner != null ? owner.GetComponent<GuardSensors>() : null;

        if (sensors == null)
        {
            if (HasLineOfSight != null) HasLineOfSight.Value = false;
            if (TimeSinceLastSeen != null)
                TimeSinceLastSeen.Value += Time.deltaTime;
            return Node.Status.Success;
        }

        bool sensed = sensors.TrySenseTarget(out GameObject sensedTarget,
                                             out Vector3 sensedPos,
                                             out bool hasLOS);

        if (sensed && hasLOS)
        {
            if (Target != null) Target.Value = sensedTarget;
            if (HasLineOfSight != null) HasLineOfSight.Value = true;
            if (LastKnownPosition != null) LastKnownPosition.Value = sensedPos;
            if (TimeSinceLastSeen != null) TimeSinceLastSeen.Value = 0f;
        }
        else
        {
            if (HasLineOfSight != null) HasLineOfSight.Value = false;
            if (TimeSinceLastSeen != null)
                TimeSinceLastSeen.Value += Time.deltaTime;
        }

        return Node.Status.Success;
    }
}



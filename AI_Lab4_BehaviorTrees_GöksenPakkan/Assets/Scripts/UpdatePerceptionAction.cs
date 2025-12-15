using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.Serialization;
using static Unity.Behavior.Node.Status;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Update Perception", description: "Updates Target/LOS/LastKnownPosition from GuardSensors", 
    story: "Update perception and write to the blackboard.", category: "Action/Sensing", id: "81bb6fa6b4c9afec2031be95c2afdcd1")]
public class UpdatePerceptionAction : Action
{
    [FormerlySerializedAs("Target")] [SerializeReference] public BlackboardVariable<GameObject>
        target;
    [FormerlySerializedAs("HasLineOfSight")] [SerializeReference] public BlackboardVariable<bool>
        hasLineOfSight;
    [FormerlySerializedAs("LastKnownPosition")] [SerializeReference] public BlackboardVariable<Vector3>
        lastKnownPosition;
    [FormerlySerializedAs("TimeSinceLastSeen")] [SerializeReference] public BlackboardVariable<float>
        timeSinceLastSeen;
    protected override Status OnStart()
    {
// Ensure we have sane defaults.
        if (timeSinceLastSeen is { Value: < 0f })
            timeSinceLastSeen.Value = 9999f;
        return Success;
    }
    protected override Status OnUpdate()
    {
        var sensors = GameObject != null ?
            GameObject.GetComponent<GuardSensors>() : null;
        if (sensors == null)
        {
// No sensors attached -> treat as "can't see anything"
            if (hasLineOfSight != null) hasLineOfSight.Value =
                false;
            if (timeSinceLastSeen != null)
                timeSinceLastSeen.Value += Time.deltaTime;
            return Success;
        }
        bool sensed = sensors.TrySenseTarget(
            out GameObject sensedTarget,
            out Vector3 sensedPos,
            out bool hasLos
        );
        if (sensed && hasLos)
        {
            if (target != null) target.Value = sensedTarget;
            if (hasLineOfSight != null) hasLineOfSight.Value =
                true;
            if (lastKnownPosition != null)
                lastKnownPosition.Value = sensedPos;
            if (timeSinceLastSeen != null)
                timeSinceLastSeen.Value = 0f;
        }
        else
        {
// Keep Target as-is (we "remember" who we were chasing),
// but mark that we don't currently have LOS.
            if (hasLineOfSight != null) hasLineOfSight.Value =
                false;
            if (timeSinceLastSeen != null)
                timeSinceLastSeen.Value += Time.deltaTime;
        }
// This node is a fast "service-like" update; it finishes immediately each tick.
        return Success;
    }
}



using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.Serialization;
using static Unity.Behavior.Node.Status;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Clear Target",description: "Clears Target and resets LOS memory.",
    story: "Forget the target and reset perception flags.", category: "Action/Sensing", id: "da2d34b2ebbfe7084d593eaf2bcaddfb")]
public class ClearTarget : Action
{
    [FormerlySerializedAs("Target")] [SerializeReference] public BlackboardVariable<GameObject>
        target;
    [FormerlySerializedAs("HasLineOfSight")] [SerializeReference] public BlackboardVariable<bool>
        hasLineOfSight;
    [FormerlySerializedAs("TimeSinceLastSeen")] [SerializeReference] public BlackboardVariable<float>
        timeSinceLastSeen;
    protected override Status OnUpdate()
    {
        if (target != null) target.Value = null;
        if (hasLineOfSight != null) hasLineOfSight.Value =
            false;
        if (timeSinceLastSeen != null) timeSinceLastSeen.Value =
            9999f;
        Debug.Log("heheheha");
        return Success;
    }
}


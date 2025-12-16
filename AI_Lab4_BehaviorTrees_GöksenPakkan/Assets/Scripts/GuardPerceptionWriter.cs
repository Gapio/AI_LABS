using UnityEngine;
using Unity.Behavior;

public class GuardPerceptionWriter : MonoBehaviour
{
    [Header("Sensing")]
    public Transform eyes;
    public float viewDistance = 15f;
    public float viewAngle = 90f;
    public LayerMask targetMask;
    public LayerMask occlusionMask;

    [Header("References")]
    public BehaviorGraph behaviorTree; 

    Blackboard Blackboard => behaviorTree != null ? behaviorTree.BlackboardReference.Blackboard : null;

    void Reset()
    {
        eyes = transform;
    }

    void Update()
    {
        if (Blackboard == null) return;

        Blackboard.TryGet("Target", out GameObject target);
        bool hasLos = false;

        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player");

        if (target != null)
        {
            hasLos = CanSee(target.transform);

            if (hasLos)
            {
                Blackboard.Set("Target", target);
                Blackboard.Set("LastKnownPosition", target.transform.position);
                Blackboard.Set("HasLastKnownPosition", true);
            }
        }

        Blackboard.Set("HasLineOfSight", hasLos);
        Blackboard.Set("Self", gameObject);
    }

    bool CanSee(Transform t)
    {
        Vector3 origin = (eyes ? eyes.position : transform.position);
        Vector3 toTarget = (t.position - origin);
        float dist = toTarget.magnitude;
        if (dist > viewDistance) return false;

        Vector3 dir = toTarget / Mathf.Max(dist, 0.0001f);
        float angle = Vector3.Angle((eyes ? eyes.forward : transform.forward), dir);
        if (angle > viewAngle * 0.5f) return false;


        // Occlusion check
        if (Physics.Raycast(origin, dir, dist, occlusionMask))

            return false;

        return true;
    }
}

public static class BlackboardExtensions
{
    public static bool TryGet<T>(this Blackboard blackboard, string name, out T value)
    {
        value = default;
        if (blackboard == null) return false;

        var v = blackboard.Variables?.Find(x => x != null && x.Name == name);
        if (v == null) return false;

        // Case 1: variable stores the value directly
        if (v.ObjectValue is T direct)
        {
            value = direct;
            return true;
        }

        // Case 2: variable stores BlackboardVariable<T>
        if (v.ObjectValue is BlackboardVariable<T> wrapped)
        {
            value = wrapped.Value;
            return true;
        }

        return false;
    }

    public static void Set<T>(this Blackboard blackboard, string name, T value)
    {
        if (blackboard == null) return;

        var v = blackboard.Variables?.Find(x => x != null && x.Name == name);
        if (v == null) return;

        if (v.ObjectValue is BlackboardVariable<T> wrapped)
        {
            wrapped.Value = value;
            return;
        }

        v.ObjectValue = value;
    }
}

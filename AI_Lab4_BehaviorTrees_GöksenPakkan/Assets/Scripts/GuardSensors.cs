using UnityEngine;

public class GuardSensors : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private string targetTag = "Player";

    [Header("View")]
    [SerializeField] private float viewDistance = 10f;
    [Range(1f, 180f)]
    [SerializeField] private float viewAngleDegrees = 90f;

    [Header("Line of Sight")]
    [SerializeField] private Transform eyes;

    [SerializeField] private LayerMask occlusionMask;

    private Transform cachedTarget;
    private Transform EyesTransform => eyes != null ? eyes : transform;

    private bool EnsureTarget()
    {
        if (cachedTarget != null) return true;

        var go = GameObject.FindGameObjectWithTag(targetTag);
        cachedTarget = go != null ? go.transform : null;
        return cachedTarget != null;
    }

    public bool TrySenseTarget(out GameObject target, out Vector3 lastKnownPosition, out bool hasLineOfSight)
    {
        target = null;
        lastKnownPosition = default;
        hasLineOfSight = false;

        if (!EnsureTarget()) return false;

        Vector3 eyePos = EyesTransform.position;
        Vector3 targetPos = cachedTarget.position + Vector3.up * 1.2f;

        Vector3 toTarget = targetPos - eyePos;
        float dist = toTarget.magnitude;
        if (dist > viewDistance) return false;

        Vector3 dir = toTarget / Mathf.Max(dist, 0.0001f);

        // Planar FOV (XZ)
        Vector3 forward = EyesTransform.forward; forward.y = 0f; forward.Normalize();
        Vector3 dirPlanar = dir; dirPlanar.y = 0f; dirPlanar.Normalize();

        float angle = Vector3.Angle(forward, dirPlanar);
        if (angle > viewAngleDegrees * 0.5f) return false;

        Debug.DrawLine(eyePos, targetPos, Color.yellow);
        Debug.Log($"dist={dist:F1} angle={angle:F1} half={viewAngleDegrees * 0.5f:F1}");


        // Occlusion (optional for now; set occlusionMask to just Default/Environment)
        Vector3 rayOrigin = eyePos + EyesTransform.forward * 0.1f;
        if (Physics.Raycast(rayOrigin, dir, out RaycastHit hit, dist, occlusionMask, QueryTriggerInteraction.Ignore))
        {
            if (!hit.transform.IsChildOf(transform) && hit.transform != cachedTarget)
                return false;
        }




        target = cachedTarget.gameObject;
        lastKnownPosition = cachedTarget.position;
        hasLineOfSight = true;
        return true;
    }
}

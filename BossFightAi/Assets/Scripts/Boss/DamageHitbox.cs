using System.Collections.Generic;
using UnityEngine;

public class DamageHitbox : MonoBehaviour
{
    [SerializeField] int damage = 20;

    readonly HashSet<IDamageable> alreadyHit = new HashSet<IDamageable>();

    void OnEnable() => alreadyHit.Clear();

    void OnTriggerEnter(Collider other)
    {
        if (!other) return;

        IDamageable dmg = other.GetComponentInParent<IDamageable>();
        if (dmg == null) return;
        if (alreadyHit.Contains(dmg)) return;

        alreadyHit.Add(dmg);

        Vector3 hitPoint = other.ClosestPoint(transform.position);
        Vector3 dir = (other.transform.position - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f) dir.Normalize();

        dmg.TryTakeDamage(new DamageInfo(damage, hitPoint, dir));
    }
}

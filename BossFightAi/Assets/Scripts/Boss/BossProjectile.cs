using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BossProjectile : MonoBehaviour
{
    [SerializeField] int damage = 15;
    [SerializeField] float lifetime = 3f;

    bool hit;

    void OnEnable()
    {
        hit = false;
        Invoke(nameof(DestroySelf), lifetime);
    }

    void OnDisable()
    {
        CancelInvoke();
    }


    void OnTriggerEnter(Collider other)
    {
        if (hit) return;

        var dmg = other.GetComponentInParent<IDamageable>();
        if (dmg == null) return;

        hit = true;

        Vector3 hitPoint = other.ClosestPoint(transform.position);
        Vector3 dir = (other.transform.position - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f) dir.Normalize();

        dmg.TryTakeDamage(new DamageInfo(damage, hitPoint, dir));
        DestroySelf();
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}

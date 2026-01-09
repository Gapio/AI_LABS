using System;
using UnityEngine;

public class BossAttackController : MonoBehaviour
{
    [Serializable]
    public class NamedHitbox
    {
        public string name;
        public GameObject hitbox;
    }

    [Header("Animator")]
    [SerializeField] Animator animator;

    [Header("Hitboxes")]
    [SerializeField] NamedHitbox[] hitboxes;

    [Header("Ranged")]
    [SerializeField] Transform projectileSpawn;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projectileSpeed = 14f;
    [SerializeField] Transform target;

    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();

        for (int i = 0; i < hitboxes.Length; i++)
            if (hitboxes[i].hitbox)
                hitboxes[i].hitbox.SetActive(false);
    }

    public void SetTarget(Transform t) => target = t;

    public void DoChainA() => animator.SetTrigger("DoChainA");
    public void DoChainB() => animator.SetTrigger("DoChainB");
    public void DoRanged() => animator.SetTrigger("DoRanged");
    public void ContinueChain() => animator.SetTrigger("ContinueChain");

    public void EnableHitbox(string hitboxName)
    {
        GameObject hb = FindHitbox(hitboxName);
        if (hb) hb.SetActive(true);
    }

    public void DisableHitbox(string hitboxName)
    {
        GameObject hb = FindHitbox(hitboxName);
        if (hb) hb.SetActive(false);
    }

    public void FireProjectile()
    {
        if (!projectilePrefab || !projectileSpawn || !target) return;

        Vector3 dir = (target.position - projectileSpawn.position);
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) dir = transform.forward;
        dir.Normalize();

        GameObject p = Instantiate(projectilePrefab, projectileSpawn.position, Quaternion.LookRotation(dir, Vector3.up));

        Rigidbody prb = p.GetComponent<Rigidbody>();
        if (prb)
        {
            Vector3 v = prb.linearVelocity;
            prb.linearVelocity = new Vector3(dir.x * projectileSpeed, v.y, dir.z * projectileSpeed);
        }
    }

    GameObject FindHitbox(string name)
    {
        for (int i = 0; i < hitboxes.Length; i++)
            if (hitboxes[i].name == name)
                return hitboxes[i].hitbox;

        return null;
    }
}

using UnityEngine;

public struct DamageInfo
{
    public int amount;
    public Vector3 hitPoint;
    public Vector3 hitDirection;

    public DamageInfo(int amount, Vector3 hitPoint, Vector3 hitDirection)
    {
        this.amount = amount;
        this.hitPoint = hitPoint;
        this.hitDirection = hitDirection;
    }
}

public interface IDamageable
{
    bool TryTakeDamage(DamageInfo info);
}

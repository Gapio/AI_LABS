using UnityEngine;

public class BossHealth : MonoBehaviour, IDamageable
{
    [SerializeField] int maxHp = 300;

    int hp;
    public int HP => hp;
    public int MaxHP => maxHp;

    public System.Action<int, int> OnHealthChanged;
    public System.Action OnDied;

    void Awake()
    {
        hp = maxHp;
        OnHealthChanged?.Invoke(hp, maxHp);
    }

    public bool TryTakeDamage(DamageInfo info)
    {
        if (hp <= 0){

            UnityEngine.SceneManagement.SceneManager.LoadScene("Win");
        };

        hp -= info.amount;
        if (hp < 0) hp = 0;

        OnHealthChanged?.Invoke(hp, maxHp);

        return true;
    }
}

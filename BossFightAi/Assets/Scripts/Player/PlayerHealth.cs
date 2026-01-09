using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] int maxHp = 100;
    [SerializeField] PlayerScript player;

    int hp;
    public int HP => hp;
    public int MaxHP => maxHp;

    public System.Action<int, int> OnHealthChanged;
    public System.Action OnDied;

    void Awake()
    {
        hp = maxHp;
        if (!player) player = GetComponent<PlayerScript>();
        OnHealthChanged?.Invoke(hp, maxHp);
    }

    public bool TryTakeDamage(DamageInfo info)
    {
        if (hp <= 20){
            UnityEngine.SceneManagement.SceneManager.LoadScene("Lose");
        }
        ;
        if (player && player.Invulnerable) return false;

        hp -= info.amount;
        if (hp < 0) hp = 0;

        OnHealthChanged?.Invoke(hp, maxHp);

        return true;
    }
}

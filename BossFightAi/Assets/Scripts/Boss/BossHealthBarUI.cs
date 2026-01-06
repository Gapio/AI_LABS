using UnityEngine;

public class BossHealthBarUI : MonoBehaviour
{
    [SerializeField] BossHealth bossHealth;
    [SerializeField] RectTransform fillRect;

    float fullWidth;

    void Awake()
    {
        fullWidth = fillRect.sizeDelta.x;
    }

    void OnEnable()
    {
        if (bossHealth != null)
            bossHealth.OnHealthChanged += UpdateBar;
    }

    void OnDisable()
    {
        if (bossHealth != null)
            bossHealth.OnHealthChanged -= UpdateBar;
    }

    void Start()
    {
        if (bossHealth != null)
            UpdateBar(bossHealth.HP, bossHealth.MaxHP);
    }

    void UpdateBar(int hp, int maxHp)
    {
        float t = (maxHp <= 0) ? 0f : (float)hp / maxHp;
        fillRect.sizeDelta = new Vector2(fullWidth * t, fillRect.sizeDelta.y);
    }
}

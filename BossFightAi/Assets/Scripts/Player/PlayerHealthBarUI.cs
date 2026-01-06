using UnityEngine;

public class PlayerHealthBarUI : MonoBehaviour
{
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] RectTransform fillRect;

    float fullWidth;

    void Awake()
    {
        fullWidth = fillRect.sizeDelta.x;
    }

    void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged += UpdateBar;
    }

    void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateBar;
    }

    void Start()
    {
        if (playerHealth != null)
            UpdateBar(playerHealth.HP, playerHealth.MaxHP);
    }

    void UpdateBar(int hp, int maxHp)
    {
        float t = (maxHp <= 0) ? 0f : (float)hp / maxHp;
        fillRect.sizeDelta = new Vector2(fullWidth * t, fillRect.sizeDelta.y);
    }
}

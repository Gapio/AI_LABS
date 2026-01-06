using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSword : MonoBehaviour
{
    [SerializeField] GameObject swordHitbox;
    [SerializeField] float windup = 0.08f;
    [SerializeField] float activeTime = 0.14f;
    [SerializeField] float recovery = 0.22f;

    bool swinging;

    void Awake()
    {
        if (swordHitbox) swordHitbox.SetActive(false);
    }

    public void OnAttack(InputValue value)
    {
        if (!value.isPressed) return;
        if (swinging) return;

        StartCoroutine(SwingRoutine());
    }

    IEnumerator SwingRoutine()
    {
        swinging = true;

        if (windup > 0f)
            yield return new WaitForSeconds(windup);

        if (swordHitbox) swordHitbox.SetActive(true);

        yield return new WaitForSeconds(activeTime);

        if (swordHitbox) swordHitbox.SetActive(false);

        if (recovery > 0f)
            yield return new WaitForSeconds(recovery);

        swinging = false;
    }
}

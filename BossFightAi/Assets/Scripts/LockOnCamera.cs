using UnityEngine;

public class LockOnCamera : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform enemy;

    [SerializeField] float distance = 5f;
    [SerializeField] float height = 2.2f;
    [SerializeField] float lookHeight = 1.2f;

    [SerializeField] Vector2 screenOffset = new Vector2(0.8f, 0.4f); // x = right, y = up

    [SerializeField] float positionSmooth = 12f;
    [SerializeField] float rotationSmooth = 16f;

    void LateUpdate()
    {
        if (!player || !enemy) return;

        Vector3 toEnemy = enemy.position - player.position;
        toEnemy.y = 0f;

        if (toEnemy.sqrMagnitude < 0.0001f)
            toEnemy = player.forward;

        Vector3 dir = toEnemy.normalized;

        Vector3 basePos = player.position - dir * distance + Vector3.up * height;

        //offset so that the player is not blocking the boss
        Vector3 rightOffset = transform.right * screenOffset.x;
        Vector3 upOffset = Vector3.up * screenOffset.y;

        Vector3 desiredPos = basePos + rightOffset + upOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, 1f - Mathf.Exp(-positionSmooth * Time.deltaTime));

        Vector3 lookPoint = enemy.position + Vector3.up * lookHeight;
        Quaternion desiredRot = Quaternion.LookRotation(lookPoint - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, 1f - Mathf.Exp(-rotationSmooth * Time.deltaTime));
    }
}

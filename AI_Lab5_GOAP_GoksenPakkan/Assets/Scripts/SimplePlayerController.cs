using UnityEngine;
using UnityEngine.InputSystem;

public class SimplePlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    private Vector2 _moveInput; // from Input System action "Move"
    void Update()
    {
        Vector3 dir = new Vector3(_moveInput.x, 0f, _moveInput.y);
        if (dir.sqrMagnitude > 1f)
            dir.Normalize();
        transform.position += dir * speed * Time.deltaTime;
        if (dir.sqrMagnitude > 0.001f)
            transform.forward = dir;
    }
    // PlayerInput (Send Messages) calls this automatically for an action named "Move"
    // Signature must match: void On<ActionName>(InputValue value)
public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

}

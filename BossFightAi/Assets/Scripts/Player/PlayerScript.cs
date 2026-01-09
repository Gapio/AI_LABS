using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerScript : MonoBehaviour
{
    [SerializeField] float maxSpeed = 6f;
    [SerializeField] float acceleration = 45f;
    [SerializeField] float deceleration = 65f;

    [SerializeField] Camera playerCamera;
    [SerializeField] Transform enemy;

    [Header("Dash")]
    [SerializeField] float dashSpeed = 14f;
    [SerializeField] float dashDuration = 0.18f;
    [SerializeField] float iFrameDuration = 0.25f;
    [SerializeField] float dashCooldown = 0.6f;

    Rigidbody rb;
    Vector2 moveInput;

    bool isDashing;
    bool invulnerable;
    float dashReadyTime;

    public bool Invulnerable => invulnerable;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        if (moveInput.sqrMagnitude > 1f) moveInput.Normalize();
    }

    public void OnDash(InputValue value)
    {
        if (!value.isPressed) return;
        if (Time.time < dashReadyTime) return;
        if (isDashing) return;

        StartCoroutine(DashRoutine());
    }


    void FixedUpdate()
    {
        if (isDashing) return;

        Vector3 fwd = playerCamera.transform.forward; fwd.y = 0f; fwd.Normalize();
        Vector3 right = playerCamera.transform.right; right.y = 0f; right.Normalize();

        Vector3 desiredDir = fwd * moveInput.y + right * moveInput.x;
        Vector3 desiredVel = desiredDir * maxSpeed;

        Vector3 v = rb.linearVelocity;
        Vector3 horiz = new Vector3(v.x, 0f, v.z);

        float rate = desiredDir.sqrMagnitude > 0.0001f ? acceleration : deceleration;
        Vector3 newHoriz = Vector3.MoveTowards(horiz, desiredVel, rate * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector3(newHoriz.x, v.y, newHoriz.z);



        Vector3 lookPos = enemy.position;
        lookPos.y = transform.position.y;

        transform.LookAt(lookPos);
    }

    void LateUpdate()
    {
        
    }

    IEnumerator DashRoutine()
    {
        dashReadyTime = Time.time + dashCooldown;

        Vector3 fwd = playerCamera.transform.forward; fwd.y = 0f; fwd.Normalize();
        Vector3 right = playerCamera.transform.right; right.y = 0f; right.Normalize();

        Vector3 dashDir = (fwd * moveInput.y + right * moveInput.x);
        if (dashDir.sqrMagnitude < 0.0001f) dashDir = transform.forward;
        dashDir.Normalize();

        isDashing = true;
        invulnerable = true;

        float dashEnd = Time.time + dashDuration;
        float iFrameEnd = Time.time + iFrameDuration;

        while (Time.time < dashEnd)
        {
            Vector3 v = rb.linearVelocity;
            rb.linearVelocity = new Vector3(dashDir.x * dashSpeed, v.y, dashDir.z * dashSpeed);
            yield return new WaitForFixedUpdate();
        }

        isDashing = false;

        while (Time.time < iFrameEnd)
            yield return null;

        invulnerable = false;
    }
}

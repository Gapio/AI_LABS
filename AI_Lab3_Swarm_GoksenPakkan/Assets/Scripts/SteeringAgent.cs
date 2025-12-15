using UnityEngine;
using System.Collections.Generic;


public class SteeringAgent : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 5f;
    public float maxForce = 10f; // Limit how "fast" we can change direction (turning radius)
    [Header("Arrive")]
    public float slowingRadius = 3f;
    [Header("Separation")]
    public float separationRadius = 1.5f;
    public float separationStrength = 5f;
    [Header("Weights")]
    public float arriveWeight = 1f;
    public float separationWeight = 1f;
    [Header("Debug")]
    public bool drawDebug = true;
    private Vector3 _velocity = Vector3.zero;
    // Optional target for Seek / Arrive
    public Transform target;
    // Static list so agents can find each other
    private static readonly List<SteeringAgent> AllAgents = new
    List<SteeringAgent>();
    private void OnEnable()
    {
    AllAgents.Add(this);
    }
    private void OnDisable()
    {
    AllAgents.Remove(this);
    }
    void Update()
    {
        Vector3 totalSteering = Vector3.zero;
// 1. Arrive (or Seek) towards target, if any
        if (target != null)
        {
            totalSteering += Arrive(target.position, slowingRadius) *
                             arriveWeight;
        }
// 2. Separation: only if there are neighbours
        if (AllAgents.Count > 1)
        {
            totalSteering += Separation(separationRadius,
                separationStrength) * separationWeight;
        }
// 3. Clamp total force (agents have finite strength)
        totalSteering = Vector3.ClampMagnitude(totalSteering, maxForce);
// 4. Integration (same as before)
        _velocity += totalSteering * Time.deltaTime;
        _velocity = Vector3.ClampMagnitude(_velocity, maxSpeed);
        transform.position += _velocity * Time.deltaTime;
        if (_velocity.sqrMagnitude > 0.0001f)
        {
            transform.forward = _velocity.normalized;
        }

    }
    
    
// -- BEHAVIOUR STUBS --
    public Vector3 Seek(Vector3 targetPos)
    {
        Vector3 toTarget = targetPos - transform.position;
// If we are already there, stop steering
        if (toTarget.sqrMagnitude < 0.0001f)
            return Vector3.zero;
// Desired Velocity: Full speed towards target
        Vector3 desired = toTarget.normalized * maxSpeed;
// Reynolds' Steering Formula
        return desired - _velocity;
    }

    private Vector3 Arrive(Vector3 targetPos, float slowRadius) {
        Vector3 toTarget = targetPos - transform.position;
        float dist = toTarget.magnitude;
        if (dist < 0.0001f)
            return Vector3.zero;
        float desiredSpeed = maxSpeed;
// Ramp down speed if within radius
        if (dist < slowingRadius)
        {
            desiredSpeed = maxSpeed * (dist / slowingRadius);
        }
        Vector3 desired = toTarget.normalized * desiredSpeed;
        return desired - _velocity;

    }

    private Vector3 Separation(float radius, float strength) { 
        Vector3 force = Vector3.zero;
        int neighbourCount = 0;
        foreach (SteeringAgent other in AllAgents)
        {
            if (other == this) continue;
            Vector3 toMe = transform.position - other.transform.position;
            float dist = toMe.magnitude;
// If they are within my personal space
            if (dist > 0f && dist < separationRadius)
            {
// Weight: 1/dist means closer neighbours push MUCH harder
                force += toMe.normalized / dist;
                neighbourCount++;
            }
        }
        if (neighbourCount > 0)
        {
            force /= neighbourCount; // Average direction
// Convert "move away" direction into a steering force
            force = force.normalized * maxSpeed;
            force = force - _velocity;
            force *= separationStrength;
        }
        return force;
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!drawDebug) return;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + _velocity);
    }

}

using UnityEngine;

public class BossFSM : MonoBehaviour
{
    enum State { Idle, Chase, Choose, Attacking, Recover }

    [SerializeField] Transform player;

    [Header("Ranges")]
    [SerializeField] float meleeRange = 3.0f;
    [SerializeField] float rangedRange = 10.0f;

    [Header("Decision")]
    [SerializeField] float thinkInterval = 0.15f;
    [SerializeField] float rangedInsteadOfChaseChance = 0.35f;

    [Header("Recovery")]
    [SerializeField] float recoverTime = 0.35f;

    [Header("Combo")]
    [SerializeField] float comboDecisionTime = 0.45f;
    [SerializeField] float comboChance = 0.8f;

    [Header("Attack Start Tracking")]
    [SerializeField] float slowFollowDuration = 0.35f;
    [SerializeField] float slowFollowSpeedMultiplier = 0.2f;

    [Header("Cooldowns")]
    [SerializeField] float rangedCooldown = 1.2f;

    [Header("Turning")]
    [SerializeField] float attackTurnSmoothTime = 0.08f;

    [Header("Reposition")]
    [SerializeField] float repositionChance = 0.25f;
    [SerializeField] float repositionDistance = 3.0f;
    [SerializeField] float repositionDuration = 0.2f;

    [SerializeField] BossAttackController attack;
    [SerializeField] BossMovement movement;
    [SerializeField] Animator animator;

    State state;

    float nextThinkTime;
    float recoverEndTime;

    float comboCheckTime;
    bool comboChecked;

    float slowFollowEndTime;

    float rangedReadyTime;

    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        TransitionTo(State.Idle);
    }

    void Start()
    {
        if (movement) movement.SetTarget(player);
        if (attack) attack.SetTarget(player);
    }

    void Update()
    {
        if (!player || !movement || !attack || !animator) return;

        if (movement.IsDashing)
            return;

        float d = Vector3.Distance(transform.position, player.position);

        switch (state)
        {
            case State.Idle:
                if (Time.time >= nextThinkTime)
                {
                    if (d > meleeRange)
                    {
                        if (CanDoRanged(d) && Random.value <= rangedInsteadOfChaseChance)
                        {
                            movement.Stop();
                            StartRanged();
                            TransitionTo(State.Attacking);
                        }
                        else
                        {
                            movement.StartChase();
                            TransitionTo(State.Chase);
                        }
                    }
                    else
                    {
                        TransitionTo(State.Choose);
                    }
                }
                break;

            case State.Chase:
                movement.TickChase();

                if (d <= meleeRange)
                {
                    movement.Stop();
                    TransitionTo(State.Choose);
                    break;
                }

                if (CanDoRanged(d) && Random.value <= rangedInsteadOfChaseChance)
                {
                    movement.Stop();
                    StartRanged();
                    TransitionTo(State.Attacking);
                }
                break;

            case State.Choose:
                ChooseAttack(d);
                break;

            case State.Attacking:
                movement.FaceTargetDamped(attackTurnSmoothTime);
                HandleAttackStartTracking();
                HandleCombo(d);

                if (!IsInAttackState())
                {
                    movement.Stop();
                    recoverEndTime = Time.time + recoverTime;
                    TransitionTo(State.Recover);
                }
                break;

            case State.Recover:

                if (Time.time >= recoverEndTime)
                {
                    TryReposition(d);
                    nextThinkTime = Time.time + thinkInterval;
                    TransitionTo(State.Idle);
                }
                break;
        }
    }

    void TransitionTo(State newState)
    {
        if (state == newState) return;

        state = newState;

        switch (state)
        {
            case State.Chase:
                movement.BeginChaseRotation();
                break;

            case State.Attacking:
                movement.BeginAttackRotation();
                break;
        }
    }

    void ChooseAttack(float d)
    {
        if (d <= meleeRange)
        {
            StartMeleeChain();
            TransitionTo(State.Attacking);
            return;
        }

        if (CanDoRanged(d))
        {
            StartRanged();
            TransitionTo(State.Attacking);
            return;
        }

        nextThinkTime = Time.time + thinkInterval;
        TransitionTo(State.Idle);
    }

    void StartMeleeChain()
    {
        movement.BeginAttackRotation();
        movement.StartSlowFollow(slowFollowSpeedMultiplier);
        slowFollowEndTime = Time.time + slowFollowDuration;

        if (Random.value < 0.5f) attack.DoChainA();
        else attack.DoChainB();

        comboCheckTime = Time.time + comboDecisionTime;
        comboChecked = false;
    }

    void StartRanged()
    {
        movement.BeginAttackRotation();
        movement.Stop();

        attack.DoRanged();
        rangedReadyTime = Time.time + rangedCooldown;

        comboChecked = true;
        slowFollowEndTime = 0f;
    }

    void HandleAttackStartTracking()
    {
        if (Time.time <= slowFollowEndTime)
            movement.TickChase();
        else
            movement.Stop();
    }

    void HandleCombo(float d)
    {
        if (comboChecked) return;
        if (Time.time < comboCheckTime) return;

        if (d <= meleeRange && Random.value <= comboChance)
            attack.ContinueChain();

        comboChecked = true;
    }

    bool CanDoRanged(float distanceToPlayer)
    {
        return Time.time >= rangedReadyTime &&
               distanceToPlayer <= rangedRange &&
               distanceToPlayer > meleeRange;
    }

    bool IsInAttackState()
    {
        var s = animator.GetCurrentAnimatorStateInfo(0);
        return s.IsTag("Attack");
    }

    void TryReposition(float distanceToPlayer)
    {
        if (distanceToPlayer > meleeRange) return;
        if (Random.value > repositionChance) return;

        movement.DashAwayFromTarget(repositionDistance, repositionDuration);
    }
}

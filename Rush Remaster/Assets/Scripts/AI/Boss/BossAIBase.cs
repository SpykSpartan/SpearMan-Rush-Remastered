using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BossAIBase : MonoBehaviour
{
    protected enum BossState
    {
        Idle,
        Chasing,
        SwordAttack,
        AreaAttack,
        Ability,
        DashAttack
    }

    [Header("References")]
    protected NavMeshAgent agent;
    protected Transform player;
    protected Animator animator;

    [Header("Detection")]
    public float detectionRange = 20f;
    public float attackRange = 3f;

    [Header("Damage")]
    public int swordDamage = 25;
    public int areaDamage = 35;
    public int dashDamage = 20;
    public LayerMask playerLayer;

    [Header("Sword Attack")]
    public float swordRange = 2.5f;
    public Vector3 swordBoxSize = new Vector3(1.5f, 2f, 1.5f);

    [Header("Area Attack")]
    public float areaRadius = 5f;

    [Header("Dash Attack")]
    public float dashSpeed = 12f;
    public float dashDistance = 6f;
    public float dashHitRadius = 1.5f;
    public float dashCooldown = 5f;
    public float dashDuration = 0.6f;

    [Header("Cooldowns")]
    public float swordCooldown = 2f;
    public float areaCooldown = 6f;
    public float abilityCooldown = 10f;
    public float statBoostCooldown = 12f;

    [Header("Action Durations")]
    public float swordDuration = 1.2f;
    public float areaDuration = 2f;
    public float abilityDuration = 2.5f;
    public float statBoostDuration = 5f;

    [Header("Stat Boost Multipliers")]
    public float damageMultiplier = 1.5f;
    public float speedMultiplier = 1.4f;

    protected float swordTimer;
    protected float areaTimer;
    protected float abilityTimer;
    protected float statBoostTimer;
    protected float dashTimer;
    protected float actionTimer;

    protected bool isBoostActive = false;

    protected float baseAgentSpeed;
    protected int baseSwordDamage;
    protected int baseAreaDamage;
    protected int baseDashDamage;

    protected BossState currentState = BossState.Idle;

    Vector3 dashDirection;
    float dashTravelled;
    HashSet<Collider> dashHitTargets = new HashSet<Collider>();


    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        baseAgentSpeed = agent.speed;
        baseSwordDamage = swordDamage;
        baseAreaDamage = areaDamage;
        baseDashDamage = dashDamage;
    }

    protected virtual void Update()
    {
        if (!player) return;

        UpdateTimers();
        HandleStatBoost();

        if (currentState != BossState.Idle &&
            currentState != BossState.Chasing)
        {
            HandleActionState();
            return;
        }

        HandleMovementAndDecisions();
    }

    void UpdateAnimator()
    {
        if (!animator) return;

        animator.SetBool("BasicMovement", currentState == BossState.Chasing);

        switch (currentState)
        {
            case BossState.SwordAttack:
                animator.SetTrigger("WeaponAttack");
                break;

            case BossState.AreaAttack:
                animator.SetTrigger("AreaAttack");
                break;

            case BossState.DashAttack:
                animator.SetTrigger("ChargingPart1");
                break;

            case BossState.Ability:
                animator.SetTrigger("EnrageAbility");
                break;
        }
    }

    protected void SetState(BossState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        UpdateAnimator();
    }

    protected void HandleMovementAndDecisions()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > detectionRange)
        {
            SetState(BossState.Idle);
            agent.ResetPath();
            return;
        }

        if (distance > attackRange)
        {
            SetState(BossState.Chasing);
            agent.SetDestination(player.position);
            return;
        }

        agent.ResetPath();

        if (abilityTimer <= 0)
            StartAbility();
        else if (dashTimer <= 0)
            StartDashAttack();
        else if (areaTimer <= 0)
            StartAreaAttack();
        else if (swordTimer <= 0)
            StartSwordAttack();
    }

    protected void HandleActionState()
    {
        actionTimer -= Time.deltaTime;

        if (currentState == BossState.DashAttack)
            PerformDashMovement();

        if (actionTimer <= 0)
            EndCurrentAction();
    }

    protected void EndCurrentAction()
    {
        agent.isStopped = false;
        SetState(BossState.Chasing);
    }

    protected void StartSwordAttack()
    {
        SetState(BossState.SwordAttack);

        swordTimer = swordCooldown;
        actionTimer = swordDuration;

        agent.isStopped = true;
        transform.LookAt(player);
    }

    protected void StartAreaAttack()
    {
        SetState(BossState.AreaAttack);

        areaTimer = areaCooldown;
        actionTimer = areaDuration;

        agent.isStopped = true;
    }

    protected void StartAbility()
    {
        SetState(BossState.Ability);

        abilityTimer = abilityCooldown;
        actionTimer = abilityDuration;

        agent.isStopped = true;

        UseAbility();
    }

    protected void StartDashAttack()
    {
        SetState(BossState.DashAttack);

        dashTimer = dashCooldown;
        actionTimer = dashDuration;

        agent.isStopped = true;

        transform.LookAt(player);

        dashDirection = transform.forward;
        dashTravelled = 0f;
        dashHitTargets.Clear();
    }

    protected void UpdateTimers()
    {
        swordTimer -= Time.deltaTime;
        areaTimer -= Time.deltaTime;
        abilityTimer -= Time.deltaTime;
        dashTimer -= Time.deltaTime;

        if (!isBoostActive)
            statBoostTimer -= Time.deltaTime;
    }

    void PerformDashMovement()
    {
        float step = dashSpeed * Time.deltaTime;

        transform.position += dashDirection * step;
        dashTravelled += step;

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            dashHitRadius,
            playerLayer
        );

        foreach (Collider hit in hits)
        {
            if (dashHitTargets.Contains(hit)) continue;

            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(dashDamage);
                dashHitTargets.Add(hit);
            }
        }

        if (dashTravelled >= dashDistance)
            actionTimer = 0f;
    }

    public void Animation_SwordHit()
    {
        SwordSlash();
    }

    public void Animation_AreaHit()
    {
        AreaAttack();
    }

    protected virtual void SwordSlash()
    {
        Vector3 center = transform.position + transform.forward * swordRange;

        Collider[] hits = Physics.OverlapBox(
            center,
            swordBoxSize * 0.5f,
            transform.rotation,
            playerLayer
        );

        foreach (Collider hit in hits)
            if (hit.TryGetComponent<IDamageable>(out var damageable))
                damageable.TakeDamage(swordDamage);
    }

    protected virtual void AreaAttack()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            areaRadius,
            playerLayer
        );

        foreach (Collider hit in hits)
            if (hit.TryGetComponent<IDamageable>(out var damageable))
                damageable.TakeDamage(areaDamage);
    }

    protected virtual void ApplyStatBoost()
    {
        isBoostActive = true;

        agent.speed = baseAgentSpeed * speedMultiplier;

        swordDamage = Mathf.RoundToInt(baseSwordDamage * damageMultiplier);
        areaDamage = Mathf.RoundToInt(baseAreaDamage * damageMultiplier);
        dashDamage = Mathf.RoundToInt(baseDashDamage * damageMultiplier);
    }

    protected virtual void EndStatBoost()
    {
        isBoostActive = false;

        agent.speed = baseAgentSpeed;

        swordDamage = baseSwordDamage;
        areaDamage = baseAreaDamage;
        dashDamage = baseDashDamage;
    }

    protected void HandleStatBoost()
    {
        if (!isBoostActive && statBoostTimer <= 0)
        {
            ApplyStatBoost();
            actionTimer = statBoostDuration;
        }

        if (isBoostActive)
        {
            actionTimer -= Time.deltaTime;

            if (actionTimer <= 0)
            {
                EndStatBoost();
                statBoostTimer = statBoostCooldown;
            }
        }
    }

    protected abstract void UseAbility();

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position + transform.forward * swordRange;
        Gizmos.DrawWireCube(center, swordBoxSize);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, areaRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, dashHitRadius);
    }
}
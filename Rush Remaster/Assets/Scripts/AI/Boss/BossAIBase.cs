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
    protected healthSystem health;

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

    [Header("Cooldowns")]
    public float swordCooldown = 2f;
    public float areaCooldown = 6f;
    public float abilityCooldown = 10f;
    public float statBoostCooldown = 12f;

    [Header("Stat Boost")]
    public float statBoostDuration = 5f;
    public float damageMultiplier = 1.5f;
    public float speedMultiplier = 1.4f;

    protected float swordTimer;
    protected float areaTimer;
    protected float abilityTimer;
    protected float statBoostTimer;
    protected float dashTimer;
    protected float dashDelayTimer = 0f;
    protected bool dashStarted = false;

    protected bool isBoostActive = false;
    protected bool isPerformingAction = false;

    protected float actionFailSafeTimer = 3f;
    protected float currentActionTime = 0f;

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
        health = GetComponent<healthSystem>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        baseAgentSpeed = agent.speed;
        baseSwordDamage = swordDamage;
        baseAreaDamage = areaDamage;
        baseDashDamage = dashDamage;
    }

    protected virtual void Update()
    {
        if (health != null && health.IsDead)
        {
            HandleDeathState();
            return;
        }

        if (!player) return;

        UpdateTimers();
        HandleStatBoost();

        if (isPerformingAction)
        {
            HandleActionState();
            return;
        }

        HandleMovementAndDecisions();
    }

    protected void HandleDeathState()
    {
        if (agent != null && !agent.isStopped)
            agent.isStopped = true;

        isPerformingAction = false;

        if (animator != null)
            animator.SetBool("BasicMovement", false);
    }

    void UpdateAnimator()
    {
        if (!animator) return;

        animator.ResetTrigger("WeaponAttack");
        animator.ResetTrigger("AreaAttack");
        animator.ResetTrigger("ChargingPart1");
        animator.ResetTrigger("EnrageAbility");

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
        currentActionTime += Time.deltaTime;

        if (currentState == BossState.DashAttack)
            PerformDashMovement();

        if (currentActionTime >= actionFailSafeTimer)
        {
            Debug.LogWarning("Action fail-safe triggered!");
            Animation_ActionComplete();
        }
    }

    protected void EndCurrentAction()
    {
        agent.isStopped = false;
        isPerformingAction = false;
        SetState(BossState.Chasing);
    }

    protected void StartSwordAttack()
    {
        if (health != null && health.IsDead) return;

        SetState(BossState.SwordAttack);

        swordTimer = swordCooldown;
        agent.isStopped = true;
        transform.LookAt(player);

        isPerformingAction = true;
        currentActionTime = 0f;
    }

    protected void StartAreaAttack()
    {
        if (health != null && health.IsDead) return;

        SetState(BossState.AreaAttack);

        areaTimer = areaCooldown;
        agent.isStopped = true;

        isPerformingAction = true;
        currentActionTime = 0f;
    }

    protected void StartAbility()
    {
        if (health != null && health.IsDead) return;

        SetState(BossState.Ability);

        abilityTimer = abilityCooldown;
        agent.isStopped = true;

        UseAbility();

        isPerformingAction = true;
        currentActionTime = 0f;
    }

    protected void StartDashAttack()
    {
        if (health != null && health.IsDead) return;

        SetState(BossState.DashAttack);

        dashTimer = dashCooldown;
        agent.isStopped = true;

        transform.LookAt(player);

        dashDirection = transform.forward;
        dashTravelled = 0f;
        dashHitTargets.Clear();

        isPerformingAction = true;
        currentActionTime = 0f;
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

    protected void PerformDashMovement()
    {
        dashDelayTimer += Time.deltaTime;

        if (!dashStarted)
        {
            if (dashDelayTimer < 0.3f) return;

            dashStarted = true;
            dashDelayTimer = 0f;
        }

        float step = dashSpeed * Time.deltaTime;

        transform.position += dashDirection * step;
        dashTravelled += step;

        Collider[] hits = Physics.OverlapSphere(transform.position, dashHitRadius, playerLayer);

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
        {
            Animation_ActionComplete();

            dashStarted = false;
            dashDelayTimer = 0f;
        }
    }

    public void Animation_ActionComplete()
    {
        EndCurrentAction();
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
        if (health != null && health.IsDead) return;

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
        if (health != null && health.IsDead) return;

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
            statBoostTimer = statBoostCooldown;
        }
    }

    protected abstract void UseAbility();

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Vector3 swordCenter = transform.position + transform.forward * swordRange;
        Gizmos.color = Color.blue;
        Gizmos.matrix = Matrix4x4.TRS(swordCenter, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, swordBoxSize);

        Gizmos.color = Color.magenta;
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.DrawWireSphere(transform.position, areaRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, dashHitRadius);
    }
}
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
        Ability
    }

    [Header("References")]
    protected NavMeshAgent agent;
    protected Transform player;

    [Header("Detection")]
    public float detectionRange = 20f;
    public float attackRange = 3f;

    [Header("Damage")]
    public int swordDamage = 25;
    public int areaDamage = 35;
    public LayerMask playerLayer;

    [Header("Sword Attack")]
    public float swordRange = 2.5f;
    public Vector3 swordBoxSize = new Vector3(1.5f, 2f, 1.5f);

    [Header("Area Attack")]
    public float areaRadius = 5f;

    [Header("Cooldowns")]
    public float swordCooldown = 2f;
    public float areaCooldown = 6f;
    public float abilityCooldown = 10f;

    [Header("Action Durations")]
    public float swordDuration = 1.2f;
    public float areaDuration = 2f;
    public float abilityDuration = 2.5f;

    protected float swordTimer;
    protected float areaTimer;
    protected float abilityTimer;
    protected float actionTimer;

    protected BossState currentState = BossState.Idle;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    protected virtual void Update()
    {
        if (!player) return;

        UpdateTimers();

        if (currentState != BossState.Idle &&
            currentState != BossState.Chasing)
        {
            HandleActionState();
            return;
        }

        HandleMovementAndDecisions();
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
        else if (areaTimer <= 0)
            StartAreaAttack();
        else if (swordTimer <= 0)
            StartSwordAttack();
    }

    protected void SetState(BossState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
    }

    protected void HandleActionState()
    {
        actionTimer -= Time.deltaTime;

        if (actionTimer <= 0)
            EndCurrentAction();
    }

    protected void EndCurrentAction()
    {
        currentState = BossState.Chasing;
        agent.isStopped = false;
    }

    protected void StartSwordAttack()
    {
        currentState = BossState.SwordAttack;
        swordTimer = swordCooldown;
        actionTimer = swordDuration;
        agent.isStopped = true;

        transform.LookAt(player);
        SwordSlash();
    }

    protected void StartAreaAttack()
    {
        currentState = BossState.AreaAttack;
        areaTimer = areaCooldown;
        actionTimer = areaDuration;
        agent.isStopped = true;

        AreaAttack();
    }

    protected void StartAbility()
    {
        currentState = BossState.Ability;
        abilityTimer = abilityCooldown;
        actionTimer = abilityDuration;
        agent.isStopped = true;

        UseAbility();
    }

    protected void UpdateTimers()
    {
        swordTimer -= Time.deltaTime;
        areaTimer -= Time.deltaTime;
        abilityTimer -= Time.deltaTime;
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
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(swordDamage);
            }
        }

        Debug.Log($"{name} hits Sword Slash");
    }

    protected virtual void AreaAttack()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            areaRadius,
            playerLayer
        );

        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(areaDamage);
            }
        }

        Debug.Log($"{name} hits Area Attack");
    }

    protected abstract void UseAbility();

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 center = transform.position + transform.forward * swordRange;
        Gizmos.DrawWireCube(center, swordBoxSize);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, areaRadius);
    }
}
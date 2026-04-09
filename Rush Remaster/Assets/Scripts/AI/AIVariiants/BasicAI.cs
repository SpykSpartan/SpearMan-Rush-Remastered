using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicAI : EnemyAIBase
{
    [Header("Movement Settings")]
    [SerializeField] private float AISpeed = 1.5f;

    [Header("Attack Settings")]
    public float hitboxActiveDuration = 0.5f;
    public List<string> attackAnimationTriggers = new List<string>();

    [Header("Parry Settings")]
    [SerializeField] private string parryTrigger = "Parried";
    [SerializeField] private float parryStunDuration = 1.0f;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    private AttackHitbox attackHitbox;
    private PlayerAttackAudioManager attackSFX;

    private bool isStunned = false;

    protected override void Awake()
    {
        base.Awake();

        if (player != null)
        {
            healthSystem playerHealth = player.GetComponent<healthSystem>();
            if (playerHealth != null)
            {
                playerHealth.OnParry += HandleParry;
            }
        }

        attackHitbox = GetComponentInChildren<AttackHitbox>(true);
        if (attackHitbox != null)
        {
            attackHitbox.parentAI = this;
            attackHitbox.DisableHitbox();
        }
        else
        {
            Debug.LogWarning("BasicAI: No AttackHitbox found in children.");
        }

        attackSFX = GetComponent<PlayerAttackAudioManager>();
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            healthSystem playerHealth = player.GetComponent<healthSystem>();
            if (playerHealth != null)
            {
                playerHealth.OnParry -= HandleParry;
            }
        }
    }

    void HandleParry(GameObject attacker)
    {
        if (attacker == null) return;

        if (attacker.transform.root != transform.root) return;

        Debug.Log("Enemy Parried!");

        isStunned = true;

        agent.isStopped = true;
        agent.ResetPath();

        if (attackHitbox != null)
            attackHitbox.DisableHitbox();

        animator.SetBool("BasicMovement", false);
        animator.SetTrigger(parryTrigger);

        StartCoroutine(StunCoroutine(parryStunDuration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);

        isStunned = false;

        agent.isStopped = false;
        agent.ResetPath();

        currentState = AIState.Chase;
    }

    protected override void Patrol()
    {
        if (isStunned) return;

        if (patrolTargets.Count == 0)
            GenerateRandomPatrolPoints();

        if (patrolTargets.Count == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolTargets.Count;
            agent.SetDestination(patrolTargets[currentPatrolIndex]);
        }

        animator.SetBool("BasicMovement", true);
    }

    protected override void Chase()
    {
        if (isStunned || player == null || holdPosition) return;

        agent.speed = AISpeed;
        Vector3 targetPosition = player.position + formationOffset;
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange + stopDistanceBuffer)
        {
            agent.isStopped = false;
            agent.SetDestination(targetPosition);
            animator.SetBool("BasicMovement", true);
        }
        else
        {
            agent.isStopped = true;
            agent.ResetPath();
            animator.SetBool("BasicMovement", false);
        }
    }

    protected override void Attack()
    {
        if (isStunned || player == null || attackAnimationTriggers.Count == 0) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (Time.time - lastAttackTime >= attackCooldown && distance <= attackRange)
        {
            lastAttackTime = Time.time;
            animator.SetBool("BasicMovement", false);

            int randomIndex = Random.Range(0, attackAnimationTriggers.Count);
            string selectedTrigger = attackAnimationTriggers[randomIndex];
            animator.SetTrigger(selectedTrigger);

            if (attackSFX != null)
                attackSFX.PlayAttack();

            if (attackHitbox != null)
            {
                attackHitbox.EnableHitbox();
                StartCoroutine(DisableHitboxAfterSeconds(hitboxActiveDuration));
            }
        }
    }

    protected override void Idle()
    {
        if (isStunned) return;

        agent.SetDestination(transform.position);
        animator.SetBool("BasicMovement", false);
    }

    protected override void TryDodge() { }

    private IEnumerator DisableHitboxAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (attackHitbox != null)
            attackHitbox.DisableHitbox();
    }

    protected override void CheckTransitions()
    {
        if (isStunned) return;

        base.CheckTransitions();
    }
}
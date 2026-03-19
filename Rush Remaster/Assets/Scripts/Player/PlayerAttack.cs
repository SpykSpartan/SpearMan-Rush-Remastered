using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Input Keys")]
    public KeyCode abilityKey = KeyCode.E;
    public KeyCode parryKey = KeyCode.Q;

    [Header("Stab Attack")]
    [SerializeField] private float stabRange = 1.25f;
    [SerializeField] private Vector3 stabBoxSize = new Vector3(1f, 1.2f, 1.5f);
    public LayerMask enemyLayer;
    public Vector3 rayOffset = new Vector3(0f, 1f, 0.5f);
    [SerializeField] private int stabDamage = 12;

    [Header("Slash Attack")]
    [SerializeField] private float slashRange = 1.5f;
    [SerializeField] private Vector3 slashBoxSize = new Vector3(2f, 1.5f, 2f);
    [SerializeField] private int slashDamage = 18;

    [Header("Parry Settings")]
    [SerializeField] private float parryTime = 0.2f;

    [Header("Cooldowns")]
    public float stabCooldown = 0.2f;
    public float slashCooldown = 0.25f;

    [Header("References")]
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerStat statSystem;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerAttackAudioManager attackSFX;

    private float lastStabTime;
    private float lastSlashTime;

    void Update()
    {
        HandleCombatInput();
    }

    void HandleCombatInput()
    {
        if (Input.GetMouseButton(0) && Time.time >= lastStabTime + stabCooldown)
        {
            StartCoroutine(PerformStab());
            lastStabTime = Time.time;
        }

        if (Input.GetMouseButton(1) && Time.time >= lastSlashTime + slashCooldown)
        {
            StartCoroutine(PerformSlashAttack());
            lastSlashTime = Time.time;
        }

        if (Input.GetKeyDown(abilityKey))
        {
            Debug.Log("Ability used");
        }

        if (Input.GetKeyDown(parryKey))
        {
            PerformParry();
        }
    }

    private IEnumerator PerformStab()
    {
        animator.SetTrigger("ForwardSpearThrust");

        yield return new WaitForSeconds(0.1f);

        bool didHit = PerformStabBox();

        if (didHit) attackSFX.PlayAttack();
        else attackSFX.PlayMiss();
    }

    private bool PerformStabBox()
    {
        bool didHit = false;

        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 center = transform.position 
                        + forward * stabRange 
                        + Vector3.up * rayOffset.y;

        Collider[] hits = Physics.OverlapBox(
            center,
            stabBoxSize * 0.5f,
            Quaternion.LookRotation(forward),
            enemyLayer
        );

        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                float modDamage = Mathf.Round(stabDamage * statSystem.damageMultiplier);
                damageable.TakeDamage((int)modDamage);

                statSystem.RegisterDamageAction();
                didHit = true;
            }
        }

        return didHit;
    }

    private IEnumerator PerformSlashAttack()
    {
        animator.SetTrigger("OverheadSpearSlash");

        yield return new WaitForSeconds(0.2f);

        bool didHit = PerformSlash();

        if (didHit) attackSFX.PlayAttack();
        else attackSFX.PlayMiss();
    }

    private bool PerformSlash()
    {
        bool didHit = false;

        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 center = transform.position + forward * slashRange;

        Collider[] hits = Physics.OverlapBox(
            center,
            slashBoxSize * 0.5f,
            Quaternion.LookRotation(forward),
            enemyLayer
        );

        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                float modDamage = Mathf.Round(slashDamage * statSystem.damageMultiplier);
                damageable.TakeDamage((int)modDamage);

                statSystem.RegisterDamageAction();
                didHit = true;
            }
        }

        return didHit;
    }

    private void PerformParry()
    {
        animator.SetTrigger("Parry");
        StartCoroutine(ParryInvulnerability());
        statSystem.RegisterTimeIncreaseAction();
    }

    private IEnumerator ParryInvulnerability()
    {
        yield return new WaitForSeconds(1f);

        healthSystem health = GetComponent<healthSystem>();
        if (health != null)
        {
            health.isInvulnerable = true;

            yield return new WaitForSeconds(parryTime + statSystem.timeIncreaseMultiplier);

            health.isInvulnerable = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Gizmos.color = Color.blue;
        Vector3 slashCenter = transform.position + forward * slashRange;
        Gizmos.matrix = Matrix4x4.TRS(slashCenter, Quaternion.LookRotation(forward), Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, slashBoxSize);

        Gizmos.color = Color.red;
        Vector3 stabCenter = transform.position + forward * stabRange + Vector3.up * rayOffset.y;
        Gizmos.matrix = Matrix4x4.TRS(stabCenter, Quaternion.LookRotation(forward), Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, stabBoxSize);
    }
}
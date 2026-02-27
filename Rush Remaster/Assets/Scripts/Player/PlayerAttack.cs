using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Input Keys")]
    public KeyCode abilityKey = KeyCode.E;
    public KeyCode parryKey = KeyCode.Q;

    [Header("Attack Settings")]
    [SerializeField] private float stabRange = 1.25f;
    public LayerMask enemyLayer;
    public Vector3 rayOffset = new Vector3(0f, 1f, 0.5f);
    [SerializeField] private int stabDamage = 12;
    [SerializeField] private float parryTime = 0.2f;

    [Header("Cooldowns")]
    public float stabCooldown = 0.5f;
    public float bashCooldown = 0.6f;

    private bool attackLocked = false;

    [Header("References")]
    public PlayerHitbox meleeHitbox;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerStat statSystem;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerAttackAudioManager attackSFX;

    void Update()
    {
        HandleCombatInput();
    }

    void HandleCombatInput()
    {
        if (Input.GetMouseButtonDown(0) && !attackLocked)
        {
            Debug.Log("Stab Attack triggered");
            StartCoroutine(PerformStab());
        }

        if (Input.GetMouseButtonDown(1) && !attackLocked)
        {
            Debug.Log("Bash Attack triggered");
            StartCoroutine(PerformBash());
        }

        if (Input.GetKeyDown(abilityKey))
        {
            Debug.Log("Ability used");
        }

        if (Input.GetKeyDown(parryKey))
        {
            Debug.Log("Parry attempt");
            PerformParry();
        }
    }

    private IEnumerator PerformStab()
    {
        attackLocked = true;
        movement.canMove = false;
        animator.SetTrigger("ForwardSpearThrust");

        yield return new WaitForSeconds(0.1f);

        bool didHit = false;

        Vector3 origin = transform.position
                        + Camera.main.transform.forward * rayOffset.z
                        + Vector3.up * 1f
                        + transform.right * rayOffset.x;

        Vector3 direction = Camera.main.transform.forward;
        direction.y = 0f;
        direction.Normalize();

        if (Physics.Raycast(origin, direction, out RaycastHit hit, stabRange, enemyLayer))
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                float modDamage = Mathf.Round(stabDamage * statSystem.damageMultiplier);
                damageable.TakeDamage((int)modDamage);
                statSystem.RegisterDamageAction();
                didHit = true;
            }
        }

        if (didHit) attackSFX.PlayAttack();
        else attackSFX.PlayMiss();

        yield return new WaitForSeconds(0.3f);

        movement.canMove = true;

        yield return new WaitForSeconds(stabCooldown);
        attackLocked = false;
    }

    private IEnumerator PerformBash()
    {
        attackLocked = true;
        movement.canMove = false;
        animator.SetTrigger("OverheadSpearSlash");

        meleeHitbox.ActivateHitbox();

        yield return new WaitForSeconds(0.4f);

        meleeHitbox.DeactivateHitbox();

        if (meleeHitbox.didHit) attackSFX.PlayAttack();
        else attackSFX.PlayMiss();

        movement.canMove = true;

        yield return new WaitForSeconds(bashCooldown);
        attackLocked = false;
    }

    public void PlayMissSFX()
    {
        if (attackSFX != null)
            attackSFX.PlayMiss();
    }

    public void PlayAttackSFX()
    {
        if (attackSFX != null)
            attackSFX.PlayAttack();
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
            Debug.Log("Parry: Player is temporarily invulnerable");
            yield return new WaitForSeconds(parryTime + statSystem.timeIncreaseMultiplier);
            health.isInvulnerable = false;
            Debug.Log("Parry: Invulnerability ended");
        }
    }
}
using System.Collections;
using UnityEngine;

public class healthSystem : MonoBehaviour, IDamageable
{
    [Header("Health + Death Settings")]
    public int baseMaxHealth = 100;
    [SerializeField] private int currentHealth;
    public int maxHealth { get; private set; }

    [Header("Boss Settings")]
    public bool isBoss = false;
    public string bossID;

    public bool IsDead => isDead;
    private bool isDead = false;

    public delegate void HealthChanged(int current, int max);
    public event HealthChanged OnHealthChanged;

    public delegate void DeathEvent();
    public event DeathEvent OnDeath;

    private PlayerStat statSystem;

    public bool isInvulnerable = false;

    private Animator animator;
    private DamageAudioManager damageSFX;

    [Header("Regeneration Settings")]
    public bool enableRegen = true;
    public float regenDelay = 3f;
    public float regenInterval = 1f;
    public int regenAmount = 3;

    private Coroutine regenCoroutine;
    private bool isHealing = false;

    [Header("Hit VFX Settings")]
    public GameObject hitVFXObject;
    public GameObject healVFX;
    public float hitVFXDuration = 0.25f;
    public float healVFXDuration = 0.25f;
    public Transform vfxSpawnPoint;

    private PlayerMovement playerMovement;
    private UnityEngine.AI.NavMeshAgent aiAgent;

    [Header("Low Health Post Processing")]
    public bool enableLowHealthPostFX = true;
    public int lowHealthThreshold = 25;
    public postprocesshotswapper postProcessController;

    private void Start()
    {
        statSystem = GetComponent<PlayerStat>();
        animator = GetComponent<Animator>();
        damageSFX = GetComponent<DamageAudioManager>();

        playerMovement = GetComponent<PlayerMovement>();
        aiAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        UpdateMaxHealth();

        if (currentHealth <= 0)
            currentHealth = maxHealth;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        CheckLowHealthPostFX();
    }

    private void Update()
    {
        if (!CompareTag("Player")) return;

        if (Input.GetKeyDown(KeyCode.R))
            TryStartRegen();
    }

    public void UpdateMaxHealth()
    {
        if (statSystem != null)
            maxHealth = Mathf.RoundToInt(baseMaxHealth * statSystem.healthMultiplier);
        else
            maxHealth = baseMaxHealth;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        CheckLowHealthPostFX();
    }

    public void TakeDamage(int amount)
    {
        if (IsDead || isInvulnerable) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        if (damageSFX != null)
            damageSFX.PlayDamageSFX();

        SpawnHitVFX();

        Debug.Log($"{gameObject.name} took {amount} damage. Health now: {currentHealth}");
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        CheckLowHealthPostFX();

        if (currentHealth <= 0)
            Die();
        else
            StopHealing("Took damage");
    }

    private void SpawnHitVFX()
    {
        if (hitVFXObject == null) return;

        ParticleSystem ps = hitVFXObject.GetComponent<ParticleSystem>();
        hitVFXObject.SetActive(true);

        if (ps != null)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play();
        }

        StartCoroutine(DisableVFX(hitVFXDuration));
    }

    private IEnumerator DisableVFX(float delay)
    {
        yield return new WaitForSeconds(delay);
        hitVFXObject.SetActive(false);
    }

    public void Heal(int amount)
    {
        if (IsDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        CheckLowHealthPostFX();

        if (currentHealth >= maxHealth)
            StopHealing("Reached max health");
    }

    public void ForceSetHealth(int amount)
    {
        currentHealth = Mathf.Clamp(amount, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        CheckLowHealthPostFX();
    }

    public void TryStartRegen()
    {
        if (!enableRegen || IsDead || currentHealth >= maxHealth) return;

        if (regenCoroutine == null)
        {
            isHealing = true;
            EnableHealVFX(true);
            regenCoroutine = StartCoroutine(RegenerateHealth());
        }
    }

    private IEnumerator RegenerateHealth()
    {
        yield return new WaitForSeconds(regenDelay);

        while (!IsDead && currentHealth < maxHealth && isHealing)
        {
            Heal(regenAmount);

            if (playerMovement != null && playerMovement.IsMoving())
            {
                StopHealing("Player moved");
                yield break;
            }

            if (aiAgent != null && aiAgent.velocity.magnitude > 0.1f)
            {
                StopHealing("AI moved");
                yield break;
            }

            yield return new WaitForSeconds(regenInterval);
        }

        StopHealing("Regen finished");
    }

    private void StopHealing(string reason = "")
    {
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }

        if (isHealing)
        {
            isHealing = false;
            EnableHealVFX(false);
            Debug.Log($"Healing stopped. Reason: {reason}");
        }
    }

    private void EnableHealVFX(bool enable)
    {
        if (healVFX == null) return;

        healVFX.SetActive(enable);

        if (enable)
        {
            ParticleSystem ps = healVFX.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.Play();
            }
        }
    }

    private void CheckLowHealthPostFX()
    {
        if (!enableLowHealthPostFX || postProcessController == null)
            return;

        bool lowHealth = currentHealth <= lowHealthThreshold;
        postProcessController.Dark = lowHealth;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} has died.");
        OnDeath?.Invoke();

        if (isBoss && !string.IsNullOrEmpty(bossID))
        {
            if (gameManager.Instance != null)
            {
                gameManager.Instance.ReportBossDefeated(bossID);
            }
            else
            {
                Debug.LogWarning("GameManager instance not found!");
            }
        }

        StopHealing("Entity died");

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (aiAgent != null)
            aiAgent.isStopped = true;

        if (animator != null)
            animator.SetTrigger("Death");

        StartCoroutine(HandleDeathSequence());
    }

    private IEnumerator HandleDeathSequence()
    {
        float deathAnimLength = 0.5f;

        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float timer = 0f;

            while (!stateInfo.IsName("Death") && timer < 3f)
            {
                yield return null;
                stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                timer += Time.deltaTime;
            }

            deathAnimLength = (stateInfo.IsName("Death")) ? stateInfo.length : 2f;
        }

        yield return new WaitForSeconds(deathAnimLength);
        Destroy(gameObject);
    }
}
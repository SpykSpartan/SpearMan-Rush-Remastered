using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronBoss : BossAIBase
{
    public GameObject explosion;
    public GameObject bash;
    public bool explode;
    public bool bashnow;
    [Header("Spear Settings")]
    public float spearRange = 4.5f;
    public Vector3 spearBoxSize = new Vector3(0.8f, 2f, 4.5f);

    protected override void SwordSlash()
    {
        Vector3 center = transform.position + transform.forward * spearRange;

        Collider[] hits = Physics.OverlapBox(
            center,
            spearBoxSize * 0.5f,
            transform.rotation,
            playerLayer
        );

        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(swordDamage);
                Instantiate(explosion, hit.gameObject.transform.position, Quaternion.identity);
            }
                

        }

        Debug.Log("Spear stab!");
    }

    protected override void AreaAttack()
    {
        base.AreaAttack();

        Debug.Log("Spear boss shockwave AoE");
    }

    protected override void ApplyStatBoost()
    {
        isBoostActive = true;

        swordDamage = Mathf.RoundToInt(baseSwordDamage * 1.35f);
        areaDamage = Mathf.RoundToInt(baseAreaDamage * 1.35f);

        Debug.Log("Spear Boss damage boosted!");
    }

    protected override void EndStatBoost()
    {
        isBoostActive = false;

        swordDamage = baseSwordDamage;
        areaDamage = baseAreaDamage;

        Debug.Log("Spear Boss boost ended");
    }

    protected override void UseAbility()
    {
        
    }

    IEnumerator deployBash()
    {
        yield return new WaitForSeconds(3.125f);

        Instantiate(bash, transform.position + new Vector3(0, 3f, 0), Quaternion.Euler(new Vector3(90, 0, 0)));
    }

    IEnumerator deployExplosion()
    {
        Vector3 pos = GameObject.Find("Spearman").transform.position;
        Instantiate(explosion, pos, Quaternion.identity);

        yield break;
    }
}
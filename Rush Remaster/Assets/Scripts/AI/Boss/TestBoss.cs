using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestBoss : BossAIBase
{
    [Header("Teleport Ability")]
    public float teleportBehindDistance = 2.5f;
    public float teleportSearchRadius = 3f;

    protected override void SwordSlash()
    {
        base.SwordSlash();
        Debug.Log("slash");
    }

    protected override void AreaAttack()
    {
        base.AreaAttack();
        Debug.Log("explosion AoE");
    }

    protected override void UseAbility()
    {
        TeleportToPlayer();
    }

    void TeleportToPlayer()
    {
        Vector3 behindPlayer =
            player.position - player.forward * teleportBehindDistance;

        if (TryTeleport(behindPlayer))
        {
            Debug.Log("Boss teleported behind player");
            return;
        }

        Vector3 nearPlayer =
            player.position + Random.insideUnitSphere * teleportBehindDistance;
        nearPlayer.y = player.position.y;

        if (TryTeleport(nearPlayer))
        {
            Debug.Log("Boss teleported near player");
            return;
        }

        Debug.Log("Teleport failed — no valid NavMesh position");
    }

    bool TryTeleport(Vector3 targetPosition)
    {
        if (NavMesh.SamplePosition(
            targetPosition,
            out NavMeshHit hit,
            teleportSearchRadius,
            NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
            transform.LookAt(player);
            return true;
        }

        return false;
    }

    protected override void ApplyStatBoost()
    {
        isBoostActive = true;

        swordDamage = Mathf.RoundToInt(baseSwordDamage * 1.25f);
        areaDamage = Mathf.RoundToInt(baseAreaDamage * 1.25f);

        Debug.Log("Damage increased by 25%");
    }

    protected override void EndStatBoost()
    {
        isBoostActive = false;

        swordDamage = baseSwordDamage;
        areaDamage = baseAreaDamage;

        Debug.Log("damage boost ended");
    }
}
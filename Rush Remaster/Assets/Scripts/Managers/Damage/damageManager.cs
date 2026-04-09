using UnityEngine;

public interface IDamageable
{
    bool TakeDamage(int amount, GameObject attacker = null);
}

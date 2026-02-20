using UnityEngine;

public class PlayerAttackAudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource sfxSource;

    [Header("Attack Sounds")]
    public AudioClip[] attackSounds;
    public AudioClip[] missSounds;

    public void PlayAttack()
    {
        if (attackSounds == null || attackSounds.Length == 0 || sfxSource == null) return;
        sfxSource.PlayOneShot(attackSounds[Random.Range(0, attackSounds.Length)]);
    }

    public void PlayMiss()
    {
        if (missSounds == null || missSounds.Length == 0 || sfxSource == null) return;
        sfxSource.PlayOneShot(missSounds[Random.Range(0, missSounds.Length)]);
    }
}
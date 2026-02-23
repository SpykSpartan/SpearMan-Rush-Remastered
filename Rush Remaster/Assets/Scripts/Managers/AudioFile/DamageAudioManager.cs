using UnityEngine;

public class DamageAudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Damage Sounds (random)")]
    public AudioClip[] damageClips;

    [Header("Single Event Sounds")]
    public AudioClip heartBeatClip;
    public AudioClip healClip;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void PlayDamageSFX()
    {
        if (audioSource == null || damageClips == null || damageClips.Length == 0)
            return;

        int index = Random.Range(0, damageClips.Length);
        audioSource.PlayOneShot(damageClips[index]);
    }

    public void PlayHeartBeat()
    {
        if (audioSource == null || heartBeatClip == null)
            return;

        audioSource.PlayOneShot(heartBeatClip);
    }

    public void PlayHeal()
    {
        if (audioSource == null || healClip == null)
            return;

        audioSource.PlayOneShot(healClip);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MenuAudio : MonoBehaviour
{
    [Header("Mouse Movement Clips")]
    public AudioClip clip1;
    public AudioClip clip2;
    public AudioClip clip3;

    [Header("Button Click")]
    public AudioClip buttonClickClip;

    [Header("Timing Settings")]
    public float minTimeBetweenSounds = 2f;
    public float maxTimeBetweenSounds = 5f;

    [Header("Mouse Sensitivity")]
    public float movementThreshold = 0.1f;

    private AudioSource audioSource;
    private Vector3 lastMousePosition;
    private float nextPlayTime;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lastMousePosition = Input.mousePosition;
        SetNextPlayTime();
    }

    void Update()
    {
        if (MouseMoved() && Time.time >= nextPlayTime)
        {
            PlayRandomClip();
            SetNextPlayTime();
        }

        lastMousePosition = Input.mousePosition;
    }

    bool MouseMoved()
    {
        return (Input.mousePosition - lastMousePosition).sqrMagnitude > movementThreshold * movementThreshold;
    }

    void PlayRandomClip()
    {
        AudioClip[] clips = { clip1, clip2, clip3 };
        AudioClip chosen = clips[Random.Range(0, clips.Length)];

        if (chosen != null)
            audioSource.PlayOneShot(chosen);
    }

    void SetNextPlayTime()
    {
        nextPlayTime = Time.time + Random.Range(minTimeBetweenSounds, maxTimeBetweenSounds);
    }

    public void PlayClickSound()
    {
        if (buttonClickClip != null)
            audioSource.PlayOneShot(buttonClickClip);
    }
}

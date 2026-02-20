using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    [Header("Footstep Audio")]
    public AudioSource footstepSource;
    public AudioClip[] walkstepClips;
    public AudioClip[] runstepClips;

    [Header("Jump / Landing")]
    public AudioClip[] jumpClips;
    public AudioClip[] landClips;

    [Header("Dash Sounds")]
    public AudioClip[] dashClips;

    [Header("Step Timing")]
    public float walkStepRate = 0.55f;
    public float sprintStepRate = 0.35f;

    private float nextStepTime;
    private PlayerMovement movement;

    private bool wasGrounded;

    private void Start()
    {
        movement = GetComponent<PlayerMovement>();
        wasGrounded = movement.controller.isGrounded;
    }

    private void Update()
    {
        bool isGrounded = movement.controller.isGrounded;

        HandleJumpLanding(isGrounded);
        HandleFootsteps(isGrounded);

        wasGrounded = isGrounded;
    }

    private void HandleJumpLanding(bool isGrounded)
    {
        if (!isGrounded && wasGrounded)
            PlayJump();

        if (isGrounded && !wasGrounded)
            PlayLand();
    }

    private void HandleFootsteps(bool isGrounded)
    {
        if (!isGrounded) return;

        float speed = movement.GetCurrentSpeed();
        if (speed <= 0.1f) return;

        bool isRunning = movement.IsSprinting();
        float stepRate = isRunning ? sprintStepRate : walkStepRate;

        if (Time.time >= nextStepTime)
        {
            PlayFootstep(isRunning);
            nextStepTime = Time.time + stepRate;
        }
    }

    private void PlayFootstep(bool isRunning)
    {
        AudioClip[] clips = isRunning ? runstepClips : walkstepClips;
        if (clips == null || clips.Length == 0 || footstepSource == null) return;

        footstepSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }

    public void PlayJump()
    {
        if (jumpClips == null || jumpClips.Length == 0 || footstepSource == null) return;
        footstepSource.PlayOneShot(jumpClips[Random.Range(0, jumpClips.Length)]);
    }

    public void PlayLand()
    {
        if (landClips == null || landClips.Length == 0 || footstepSource == null) return;
        footstepSource.PlayOneShot(landClips[Random.Range(0, landClips.Length)]);
    }

    public void PlayDash()
    {
        if (dashClips == null || dashClips.Length == 0 || footstepSource == null) return;
        footstepSource.PlayOneShot(dashClips[Random.Range(0, dashClips.Length)]);
    }
}
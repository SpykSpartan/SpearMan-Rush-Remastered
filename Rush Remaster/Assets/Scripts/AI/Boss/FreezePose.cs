using UnityEngine;

public class FreezePose : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Freeze();
        }
    }

    public void Freeze()
    {
        animator.speed = 0f;
    }

    public void Unfreeze()
    {
        animator.speed = 1f; // resumes animation if needed
    }
}
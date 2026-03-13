using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeRoamCam : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float fastSpeed = 30f;
    public float climbSpeed = 8f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2.5f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 rot = transform.eulerAngles;
        rotationX = rot.y;
        rotationY = rot.x;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleSpeedScroll();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.deltaTime;

        rotationX += mouseX;
        rotationY -= mouseY;
        rotationY = Mathf.Clamp(rotationY, -90f, 90f);

        transform.rotation = Quaternion.Euler(rotationY, rotationX, 0f);
    }

    void HandleMovement()
    {
        float speed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : moveSpeed;

        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            move += transform.forward;

        if (Input.GetKey(KeyCode.S))
            move -= transform.forward;

        if (Input.GetKey(KeyCode.A))
            move -= transform.right;

        if (Input.GetKey(KeyCode.D))
            move += transform.right;

        if (Input.GetKey(KeyCode.Space))
            move += Vector3.up;

        if (Input.GetKey(KeyCode.LeftControl))
            move -= Vector3.up;

        transform.position += move * speed * Time.deltaTime;
    }

    void HandleSpeedScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            moveSpeed += scroll * 10f;
            moveSpeed = Mathf.Clamp(moveSpeed, 2f, 100f);
        }
    }
}

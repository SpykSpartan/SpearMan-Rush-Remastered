using UnityEngine;

public class CinematicCam : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Controls")]
    public KeyCode startKey = KeyCode.R;

    [Header("Rotation Settings")]
    public float rotationSpeed = 10f;
    public int numberOfRotations = 2;

    [Header("Vertical Movement")]
    public float verticalSpeed = 0.5f;

    [Header("Stop Settings")]
    public Transform stopPoint;

    private float totalRotation = 0f;
    private float targetRotation;

    private bool finishedRotation = false;
    private bool startedRotation = false;

    void Start()
    {
        targetRotation = numberOfRotations * 360f;
    }

    void Update()
    {
        if (target == null) return;

        if (!startedRotation && Input.GetKeyDown(startKey))
        {
            startedRotation = true;
        }

        if (!startedRotation) return;

        if (!finishedRotation)
        {
            float rotationStep = rotationSpeed * Time.deltaTime;

            transform.RotateAround(target.position, Vector3.up, rotationStep);
            transform.position += Vector3.up * verticalSpeed * Time.deltaTime;

            totalRotation += rotationStep;

            if (totalRotation >= targetRotation)
            {
                finishedRotation = true;
            }
        }
        else
        {
            if (stopPoint != null)
            {
                transform.position = Vector3.Lerp(
                    transform.position,
                    stopPoint.position,
                    Time.deltaTime * 1.5f
                );

                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    stopPoint.rotation,
                    Time.deltaTime * 1.5f
                );
            }
        }
    }
}
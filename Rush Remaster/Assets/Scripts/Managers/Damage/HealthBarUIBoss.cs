using UnityEngine;

public class HealthBarUIBoss : MonoBehaviour
{
    [Header("Target")]
    public healthSystem targetHealth;

    [Header("UI Bars")]
    public RectTransform leftBar;
    public RectTransform rightBar;

    [Header("Settings")]
    public float smoothSpeed = 5f;

    private float currentPercent = 1f;
    private float targetPercent = 1f;

    void Start()
    {
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged += UpdateHealthBar;

            UpdateHealthBar(targetHealthMaxSafe(), targetHealthMaxSafe());
        }
    }

    void OnDestroy()
    {
        if (targetHealth != null)
            targetHealth.OnHealthChanged -= UpdateHealthBar;
    }

    void UpdateHealthBar(int current, int max)
    {
        targetPercent = (float)current / max;
    }

    void Update()
    {
        currentPercent = Mathf.Lerp(currentPercent, targetPercent, Time.deltaTime * smoothSpeed);

        UpdateBars();
    }

    void UpdateBars()
    {
        float halfWidth = currentPercent;

        leftBar.localScale = new Vector3(halfWidth, 1f, 1f);

        rightBar.localScale = new Vector3(halfWidth, 1f, 1f);
    }

    int targetHealthMaxSafe()
    {
        return (targetHealth != null && targetHealth.maxHealth > 0) ? targetHealth.maxHealth : 1;
    }
}
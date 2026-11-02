using UnityEngine;

/// <summary>
/// A reusable charge meter. Drains while an ability is in use and refills after a short delay.
/// Values are in seconds of use, so maxCharge = 5 means five seconds.
/// </summary>
[System.Serializable]
public class ChargeMeter
{
    [SerializeField] private float maxCharge = 5f;
    [SerializeField] private float regenPerSecond = 1f;
    [SerializeField] private float regenDelay = 0.75f;
    [SerializeField, Range(0f, 1f)] private float minToStart = 0.15f;

    private float current;
    private float lastUseTime = float.NegativeInfinity;

    public float Normalized => maxCharge <= 0f ? 0f : current / maxCharge;
    public bool IsEmpty => current <= 0f;
    public bool CanStart => Normalized >= minToStart;

    public void Refill() => current = maxCharge;

    public void Drain(float amount)
    {
        current = Mathf.Max(0f, current - amount);
        lastUseTime = Time.time;
    }

    public void Regenerate(float deltaTime)
    {
        if (Time.time - lastUseTime < regenDelay) return;
        current = Mathf.Min(maxCharge, current + regenPerSecond * deltaTime);
    }
}
using UnityEngine;

public enum PowerUpType
{
    Machinegun,
    Shotgun,
    Grenade,
    HealthPack
}

public class PowerUp : MonoBehaviour
{
    public PowerUpType powerUpType;
    public float bobHeight = 0.5f;
    public float bobSpeed = 2f;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Bobbing effect
        transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobHeight;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HotbarManager.instance.AddPowerUp(powerUpType);
            Destroy(gameObject);
        }
    }
}
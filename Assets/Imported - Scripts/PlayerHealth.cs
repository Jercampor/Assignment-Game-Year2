using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;
    public Slider healthBar;
    public float invincibilityTime = 0.5f;
    private float invincibilityTimer = 0f;
    private PlayerController playerController;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (invincibilityTimer > 0f)
            invincibilityTimer -= Time.deltaTime;
    }

    public void TakeDamage(int damage)
    {
        if (invincibilityTimer > 0f || playerController.isDashing) return;
        currentHealth -= damage;
        healthBar.value = currentHealth;
        invincibilityTimer = invincibilityTime;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        healthBar.value = currentHealth;
    }

    void Die()
    {
        GameManager.instance.GameOver();
    }
    
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}


using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int scoreValue = 10;
    public int damage = 1;
    public int maxHealth = 3;
    private int health;
    public float moveSpeed = 2f;
    public bool immuneToDash = false;
    private Transform player; 
    public Slider healthBar;
    public float separationDistance = 1.5f;
    public float separationForce = 2f;
    private Rigidbody rb;
    private Animator animator;
    public ParticleSystem particle;
    public float mapBoundaryX = 105f;
    public float mapBoundaryZ = 129f;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").transform;
        health = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = health;
        damageTimer = damageCooldown;
        
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        
        // Separation from other enemies
        Collider[] nearby = Physics.OverlapSphere(transform.position, separationDistance);
        foreach (Collider col in nearby)
        {
            if (col.gameObject != gameObject && col.GetComponent<Enemy>() != null)
            {
                Vector3 pushDir = transform.position - col.transform.position;
                rb.AddForce(pushDir.normalized * separationForce);
            }
        }
        
        if (transform.position.x > mapBoundaryX || transform.position.x < -mapBoundaryX ||
            transform.position.z > mapBoundaryZ || transform.position.z < -mapBoundaryZ)
        {
            Destroy(gameObject);
        }
        
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        AudioManager.instance.PlayEnemyHit();
        healthBar.value = health;
        animator.SetTrigger("Hit");
        if (health <= 0)
        {
            ParticleSystem enemydie = Instantiate(particle, transform.position, Quaternion.identity);
            enemydie.Play();
            Destroy(enemydie.gameObject, enemydie.main.duration);
            GameManager.instance.AddScore(scoreValue);
            Destroy(gameObject);
            
        }
    }
    
    private float damageTimer = 0f;
    public float damageCooldown = 1f;

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageCooldown)
            {
                collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
                damageTimer = 0f;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            damageTimer = damageCooldown;
        }
    }
    
    
}
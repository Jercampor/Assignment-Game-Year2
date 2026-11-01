using UnityEngine;
using UnityEngine.UI;

public class ShooterEnemy : MonoBehaviour
{
    public int damage = 1;
    public int maxHealth = 4;
    private int health;
    public float moveSpeed = 2f;
    public float stopDistance = 8f;
    public float fireRate = 2f;
    private float fireTimer;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public bool immuneToDash = false;
    private Transform player;
    public Slider healthBar;
    public float separationDistance = 1.5f;
    public float separationForce = 2f;
    private Rigidbody rb;
    private Animator animator;

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
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > stopDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
        else
        {
            // Stop and face player
            Vector3 lookDir = player.position - transform.position;
            lookDir.y = 0f;
            if (lookDir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(lookDir);

            // Fire at player
            fireTimer += Time.deltaTime;
            if (fireTimer >= fireRate)
            {
                Shoot();
                fireTimer = 0f;
            }
        }
        
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
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody>().linearVelocity = firePoint.forward * 10f;
        Destroy(bullet, 3f);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.value = health;
        animator.SetTrigger("Hit");
        if (health <= 0)
        {
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
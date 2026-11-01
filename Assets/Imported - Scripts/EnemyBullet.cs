using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public int damage = 1;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
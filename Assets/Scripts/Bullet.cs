using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    void Start()
    {
        Collider[] bullets = FindObjectsByType<Collider>(FindObjectsSortMode.None);
        foreach (Collider col in bullets)
        {
            if (col.CompareTag("Bullet"))
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), col);
            }
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(1);
        }
        Destroy(gameObject);
    }
}
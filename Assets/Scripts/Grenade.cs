using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float explosionRadius = 5f;
    public int explosionDamage = 999;
    public float fuseTime = 2f;
    private float timer;
    private bool hasExploded = false;
    public Material explosionMaterial;


    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= fuseTime)
        {
            Explode();
        }
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        AudioManager.instance.PlayExplosion();
        Camera.main.GetComponent<CameraFollow>().Shake(0.5f, 1f);
        Debug.Log("Shake called!");
        
        GameObject explosion = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        explosion.transform.position = transform.position;
        explosion.transform.localScale = Vector3.one * explosionRadius * 2f;

        Renderer rend = explosion.GetComponent<Renderer>();
        rend.material = explosionMaterial;

        Destroy(explosion.GetComponent<Collider>());
        Destroy(explosion, 0.2f);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider col in colliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
                enemy.TakeDamage(explosionDamage);

            ShooterEnemy shooter = col.GetComponent<ShooterEnemy>();
            if (shooter != null)
                shooter.TakeDamage(explosionDamage);
        }

        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) return;
        Explode();
    }
}
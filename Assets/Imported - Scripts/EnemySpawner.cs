using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;
    public int maxEnemies = 30;

    public GameObject basicEnemy;
    public GameObject fastEnemy;
    public GameObject tankyEnemy;
    public GameObject shooterEnemy;
    public float spawnInterval = 2f;
    public float minimumSpawnInterval = 0.3f;
    public float spawnIntervalDecrease = 0.2f;
    private float timer;

    void Awake()
    {
        instance = this;
    }

    public void IncreaseSpawnRate()
    {
        spawnInterval = Mathf.Max(spawnInterval - spawnIntervalDecrease, minimumSpawnInterval);
        Debug.Log("Spawn rate increased! New interval: " + spawnInterval);
    }

    GameObject GetRandomEnemy()
    {
        int roll = Random.Range(0, 10);
        if (roll < 4) return basicEnemy;
        else if (roll < 7) return fastEnemy;
        else if (roll < 9) return shooterEnemy;
        else return tankyEnemy;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        if (FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length >= maxEnemies) return;
        Vector3 playerPos = GameObject.FindWithTag("Player").transform.position;
        Vector3 spawnPos;
        int side = Random.Range(0, 4);

        switch (side)
        {
            case 0:
                spawnPos = new Vector3(playerPos.x + Random.Range(-15f, 15f), 1.3f, playerPos.z + 20f);
                break;
            case 1:
                spawnPos = new Vector3(playerPos.x + Random.Range(-15f, 15f), 1.3f, playerPos.z - 20f);
                break;
            case 2:
                spawnPos = new Vector3(playerPos.x - 20f, 1.3f, playerPos.z + Random.Range(-15f, 15f));
                break;
            default:
                spawnPos = new Vector3(playerPos.x + 20f, 1.3f, playerPos.z + Random.Range(-15f, 15f));
                break;
        }

        Instantiate(GetRandomEnemy(), spawnPos, Quaternion.identity);
    }
}
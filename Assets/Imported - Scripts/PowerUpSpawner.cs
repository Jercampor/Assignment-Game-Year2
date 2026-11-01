using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject[] powerUpPrefabs;
    public float spawnInterval = 10f;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnPowerUp();
            timer = 0f;
        }
    }

    void SpawnPowerUp()
    {
        Vector3 playerPos = GameObject.FindWithTag("Player").transform.position;
        int side = Random.Range(0, 4);
        Vector3 spawnPos;

        switch (side)
        {
            case 0:
                spawnPos = new Vector3(playerPos.x + Random.Range(-15f, 15f), 1f, playerPos.z + 20f);
                break;
            case 1:
                spawnPos = new Vector3(playerPos.x + Random.Range(-15f, 15f), 1f, playerPos.z - 20f);
                break;
            case 2:
                spawnPos = new Vector3(playerPos.x - 20f, 1f, playerPos.z + Random.Range(-15f, 15f));
                break;
            default:
                spawnPos = new Vector3(playerPos.x + 20f, 1f, playerPos.z + Random.Range(-15f, 15f));
                break;
        }

        GameObject randomPowerUp = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
        Instantiate(randomPowerUp, spawnPos, Quaternion.identity);
    }
}
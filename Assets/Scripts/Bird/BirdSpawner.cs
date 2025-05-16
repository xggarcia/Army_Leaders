using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    [Header("Bird Settings")]
    public GameObject birdPrefab;
    public float flightSpeed = 20f;

    [Header("Spawn Timing")]
    public float spawnIntervalMin = 10f;
    public float spawnIntervalMax = 15f;

    [Header("Flight Path Settings")]
    public Vector3 mapCenter = Vector3.zero;
    public float spawnDistance = 60f;     // Distance from map center
    public float yMin = 25f;              // Vertical spawn range
    public float yMax = 40f;

    private float nextSpawnTime;

    void Start()
    {
        ScheduleNextSpawn();
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnBird();
            ScheduleNextSpawn();
        }
    }

    void ScheduleNextSpawn()
    {
        nextSpawnTime = Time.time + Random.Range(spawnIntervalMin, spawnIntervalMax);
    }

    void SpawnBird()
    {
        Vector3 spawnPos = Vector3.zero;
        Vector3 exitPos = Vector3.zero;
        float yPos = Random.Range(yMin, yMax);

        int side = Random.Range(0, 4); // 0 = Left, 1 = Right, 2 = Top, 3 = Bottom

        switch (side)
        {
            case 0: // Left
                spawnPos = mapCenter + new Vector3(-spawnDistance, yPos, 0f);
                exitPos = mapCenter + new Vector3(spawnDistance, yPos, 0f);
                break;

            case 1: // Right
                spawnPos = mapCenter + new Vector3(spawnDistance, yPos, 0f);
                exitPos = mapCenter + new Vector3(-spawnDistance, yPos, 0f);
                break;

            case 2: // Top
                spawnPos = mapCenter + new Vector3(0f, yPos, spawnDistance);
                exitPos = mapCenter + new Vector3(0f, yPos, -spawnDistance);
                break;

            case 3: // Bottom
                spawnPos = mapCenter + new Vector3(0f, yPos, -spawnDistance);
                exitPos = mapCenter + new Vector3(0f, yPos, spawnDistance);
                break;
        }

        GameObject bird = Instantiate(birdPrefab, spawnPos, Quaternion.identity);
        bird.transform.LookAt(exitPos);
        bird.transform.Rotate(0f, 180f, 0f); // Flip if facing backward

        bird.AddComponent<BirdFlyAcross>().Init(exitPos, flightSpeed);
    }
}

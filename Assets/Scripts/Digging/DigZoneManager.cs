using UnityEngine;
using System.Collections.Generic;

public class DigZoneManager : MonoBehaviour
{
    [Header("Dig Zone Area")]
    public Vector2 xBounds;
    public Vector2 zBounds;
    public float yHeight = 0.2f;

    [Header("Spawn Settings")]
    public GameObject commonDigSpotPrefab;
    public GameObject rareDigSpotPrefab;
    public GameObject epicDigSpotPrefab;
    public GameObject legendaryDigSpotPrefab;
    public int maxDigSpots = 1;
    public RarityManager rarityManager;

    private float spawnTimer = 0f;
    private float spawnInterval = 1f;
    private List<DigSpot> activeSpots = new List<DigSpot>();

    void Start()
    {
    }
    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;

            int currentActive = CountActiveDiggableSpots();
            float spawnChance = 0f;

            if (currentActive < 3)
                spawnChance = 1f; // 100%
            else if (currentActive < 5)
                spawnChance = 0.5f; // 50%
            else if (currentActive < 10)
                spawnChance = 0.20f; // 15%

            if (currentActive < 10 && Random.value < spawnChance)
            {
                SpawnNewDigSpot("Red"); // or alternate team logic if needed
            }
        }
    }

    public void SpawnNewDigSpot(string team)
    {
        Vector3 newPos = new Vector3(
            Random.Range(xBounds.x, xBounds.y),
            yHeight,
            Random.Range(zBounds.x, zBounds.y)
        );

        Rarity rarity = rarityManager.GetRandomRarity();

        // ✅ This method must return the correct prefab
        GameObject prefabToUse = GetPrefabForRarity(rarity);

        if (prefabToUse == null)
        {
            Debug.LogError($"❌ No prefab assigned for rarity {rarity}");
            return;
        }

        GameObject newSpot = Instantiate(prefabToUse, newPos, Quaternion.identity);
        DigSpot digScript = newSpot.GetComponent<DigSpot>();

        if (digScript == null)
        {
            Debug.LogError($"❌ Prefab {prefabToUse.name} does not contain DigSpot script");
            return;
        }

        digScript.manager = this;
        digScript.ownerTeam = team;
        digScript.rarity = rarity;

        Debug.Log($"✅ Spawned DigSpot of rarity {rarity} using prefab {prefabToUse.name}");

        activeSpots.Add(digScript);
    }

    private GameObject GetPrefabForRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                return commonDigSpotPrefab;
            case Rarity.Rare:
                return rareDigSpotPrefab;
            case Rarity.Epic:
                return epicDigSpotPrefab;
            case Rarity.Legendary:
                return legendaryDigSpotPrefab;
            default:
                Debug.LogWarning("⚠️ Invalid rarity provided: " + rarity);
                return null;
        }
    }

    public bool IsPlayerNearAnySpot(Vector3 playerPos, out DigSpot matchedSpot, float maxDistance)
    {
        foreach (var spot in activeSpots)
        {
            if (spot != null && !spot.isCompleted && spot.IsPlayerNear(playerPos, maxDistance))
            {
                matchedSpot = spot;
                return true;
            }
        }

        matchedSpot = null;
        return false;
    }

    public void SpotCompleted(DigSpot spot)
    {
        if (spot != null)
        {
            spot.isCompleted = true;

            if (activeSpots.Contains(spot))
            {
                activeSpots.Remove(spot);
                Destroy(spot.gameObject);
                SpawnNewDigSpot(spot.ownerTeam);
            }
        }
    }
    private int CountActiveDiggableSpots()
    {
        int count = 0;
        foreach (var spot in activeSpots)
        {
            if (spot != null && !spot.isCompleted)
                count++;
        }
        return count;
    }

}

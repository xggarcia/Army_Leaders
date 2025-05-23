using UnityEngine;
using System.Collections.Generic;

public class DigZoneManager : MonoBehaviour
{
    [Header("Dig Zone Area")]
    public Vector2 xBounds;
    public Vector2 zBounds;
    public float yHeight = 0.2f;

    [Header("Spawn Settings")]
    public GameObject digSpotPrefab;
    public int maxDigSpots = 1;
    public int digsPerSpot = 3;

    private List<DigSpot> activeSpots = new List<DigSpot>();

    void Start()
    {
        for (int i = 0; i < maxDigSpots; i++)
            SpawnNewDigSpot();
    }

    public void SpawnNewDigSpot()
    {
        Vector3 newPos = new Vector3(
            Random.Range(xBounds.x, xBounds.y),
            yHeight,
            Random.Range(zBounds.x, zBounds.y)
        );

        GameObject newSpot = Instantiate(digSpotPrefab, newPos, Quaternion.identity);
        DigSpot digScript = newSpot.AddComponent<DigSpot>();
        digScript.manager = this;
        digScript.requiredDigs = digsPerSpot;

        activeSpots.Add(digScript);
    }

    public bool IsPlayerNearAnySpot(Vector3 playerPos, out DigSpot matchedSpot, float maxDistance)
    {
        foreach (var spot in activeSpots)
        {
            if (spot != null && spot.IsPlayerNear(playerPos, maxDistance))
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
        if (activeSpots.Contains(spot))
        {
            activeSpots.Remove(spot);
            Destroy(spot.gameObject);
            SpawnNewDigSpot();
        }
    }

    public void ReduceRequiredDigsForTeam(string team)
    {
        foreach (var spot in activeSpots)
        {
            if (spot != null && spot.ownerTeam == team)
            {
                spot.requiredDigs = Mathf.Max(1, spot.requiredDigs - 1);
            }
        }
    }

}

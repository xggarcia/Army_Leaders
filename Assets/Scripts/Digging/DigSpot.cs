using UnityEngine;

public class DigSpot : MonoBehaviour
{
    public DigZoneManager manager;
    public int requiredDigs = 3;
    private int currentDigs = 0;

    public string ownerTeam; // "Red" or "Blue"


    public bool IsPlayerNear(Vector3 playerPos, float maxDistance)
    {
        Vector3 flatPlayer = new Vector3(playerPos.x, 0, playerPos.z);
        Vector3 flatSpot = new Vector3(transform.position.x, 0, transform.position.z);
        return Vector3.Distance(flatPlayer, flatSpot) <= maxDistance;
    }

    public void RegisterDig()
    {
        currentDigs++;
        if (currentDigs >= requiredDigs)
        {
            manager.SpotCompleted(this);
        }
    }
}

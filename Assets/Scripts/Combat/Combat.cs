using UnityEngine;
using System.Collections.Generic;

public class Combat : MonoBehaviour
{
    [Header("Z-Axis Movement Range")]
    public float minZ = -10f;
    public float maxZ = 10f;
    public float maxMoveSpeed = 2f; // Reduced maximum movement speed

    [Header("Current Stats")]
    public TeamStats redTeamStats;
    public TeamStats blueTeamStats;

    [Header("Object Influence Mapping")]
    public List<StatModifier> objectModifiers; // Each prefab or object will have an effect

    private float currentZ = 0f;

    void Start()
    {
        currentZ = transform.position.z;
    }

    void Update()
    {
        float scoreRed = redTeamStats.GetScore();
        float scoreBlue = blueTeamStats.GetScore();
        float scoreDiff = Mathf.Abs(scoreRed - scoreBlue);

        if (scoreRed != scoreBlue)
        {
            float direction = Mathf.Sign(scoreBlue - scoreRed); // +1 if Blue is stronger, -1 if Red is stronger

            // Calculate movement speed based on score difference (slower scaling)
            float speed = Mathf.Clamp(scoreDiff * 0.25f, 0.05f, maxMoveSpeed);
            currentZ += direction * speed * Time.deltaTime;
            currentZ = Mathf.Clamp(currentZ, minZ, maxZ);
            transform.position = new Vector3(transform.position.x, transform.position.y, currentZ);

            if (currentZ <= minZ || currentZ >= maxZ)
            {
                EndGame(currentZ >= maxZ ? "Blue" : "Red");
            }
        }
    }

    public void ImproveStats(GameObject dugObject, string team)
    {
        foreach (StatModifier mod in objectModifiers)
        {
            if (mod.prefab == dugObject)
            {
                if (team == "Red")
                    redTeamStats.ApplyModifier(mod);
                else if (team == "Blue")
                    blueTeamStats.ApplyModifier(mod);

                return;
            }
        }

        Debug.LogWarning("Dug object has no associated stat modifier.");
    }

    private void EndGame(string winner)
    {
        Debug.Log($"Game Over! {winner} team wins!");
        // Add end-game logic here (UI, freeze gameplay, etc.)
    }
}

[System.Serializable]
public class TeamStats
{
    public int power;
    public int defense;
    public int speed;

    public void ApplyModifier(StatModifier mod)
    {
        power += mod.powerBoost;
        defense += mod.defenseBoost;
        speed += mod.speedBoost;
    }

    public float GetScore()
    {
        // Custom scoring formula
        return power * 1.5f + defense * 1.2f + speed;
    }
}

[System.Serializable]
public class StatModifier
{
    public GameObject prefab;
    public int powerBoost;
    public int defenseBoost;
    public int speedBoost;
}

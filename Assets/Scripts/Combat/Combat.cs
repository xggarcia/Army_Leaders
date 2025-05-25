using UnityEngine;
using System.Collections.Generic;

public class Combat : MonoBehaviour
{
    [Header("Z-Axis Movement Range")]
    public float minZ = -10f;
    public float maxZ = 10f;
    public float maxMoveSpeed = 5f;

    [Header("Current Stats")]
    public TeamStats redTeamStats;
    public TeamStats blueTeamStats;

    [Header("Object Influence Mapping")]
    public List<StatModifier> objectModifiers;

    [Header("Base Reference")]
    public BaseHealthController baseHealthController;

    [Header("Special Objects")]
    public GameObject specialDigObject;
    public GameObject healingDigObject;
    public int healingAmount = 10;

    private float currentZ = 0f;
    private bool isDamagingBase = false;
    private string winningTeam = "";
    private float damageTimer = 0f;

    void Start()
    {
        currentZ = transform.position.z;
    }

    void Update()
    {
        if (isDamagingBase)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= 1f)
            {
                damageTimer = 0f;
                int damage = (winningTeam == "Red") ? redTeamStats.power : blueTeamStats.power;
                if (winningTeam == "Red")
                    baseHealthController.RemoveHealth(damage, "blue");
                else if (winningTeam == "Blue")
                    baseHealthController.RemoveHealth(damage, "red");
            }
            return;
        }

        float scoreRed = redTeamStats.GetScore();
        float scoreBlue = blueTeamStats.GetScore();
        float scoreDiff = Mathf.Abs(scoreRed - scoreBlue);

        if (scoreRed != scoreBlue)
        {
            float direction = Mathf.Sign(scoreBlue - scoreRed);
            float speed = Mathf.Clamp(scoreDiff, 0.1f, maxMoveSpeed);
            currentZ += direction * speed * Time.deltaTime;
            currentZ = Mathf.Clamp(currentZ, minZ, maxZ);
            transform.position = new Vector3(transform.position.x, transform.position.y, currentZ);

            if (currentZ <= minZ || currentZ >= maxZ)
            {
                winningTeam = (currentZ >= maxZ) ? "Blue" : "Red";
                Debug.Log($"Game Over! {winningTeam} team wins!");
                isDamagingBase = true;
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

                break;
            }
        }

        if (dugObject.name == healingDigObject.name)
        {
            if (team == "Red")
                baseHealthController.AddHealth(healingAmount, "red");
            else if (team == "Blue")
                baseHealthController.AddHealth(healingAmount, "blue");

            Debug.Log($"Healing object collected by {team} team. Base healed by {healingAmount}.");
        }
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

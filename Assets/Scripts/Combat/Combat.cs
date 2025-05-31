using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

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
    public GameObject specialDigObject2;

    public List<HealingObject> healingObjects;  // NEW

    [Header("UI Events")]
    public UnityEvent<string> OnGameOver;

    [Header("Bomb")]
    public GameObject bomb_epic;
    public GameObject bomb_legendeary; 

    [Header("Explosion")]
    public GameObject explosion;
    public Vector3 REDexplosionCordinates;
    public Vector3 BLUEexplosionCordinates;

    private float currentZ = 0f;
    private bool isDamagingBase = false;
    private string winningTeam = "";
    private float damageTimer = 0f;

    public bool IsDamagingBase => isDamagingBase;
    public string CurrentWinningTeam => winningTeam;
    public float TankPosition => currentZ;
    public float TankProgress => Mathf.InverseLerp(minZ, maxZ, currentZ);

    void Start()
    {
        currentZ = transform.position.z;
    }

    void Update()
    {
        // Damage base if touching it
        if (isDamagingBase)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= 1f)
            {
                damageTimer = 0f;
                int damage = (winningTeam == "Red") ? redTeamStats.power : blueTeamStats.power;

                if (winningTeam == "Red")
                {
                    baseHealthController.RemoveHealth(damage, "blue");
                    DamageAnimation("blue");
                }
                else if (winningTeam == "Blue")
                {
                    baseHealthController.RemoveHealth(damage, "red");
                    DamageAnimation("red");
                }
            }
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

            // Winner is only declared if tank reaches far edge
            if (currentZ <= minZ)
            {
                winningTeam = "Red";
            }
            else if (currentZ >= maxZ)
            {
                winningTeam = "Blue";
            }
            else
            {
                winningTeam = "";
            }

            isDamagingBase = (winningTeam != "");
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

        foreach (HealingObject heal in healingObjects)
        {
            if (dugObject.name == heal.prefab.name)
            {
                if (team == "Red")
                    baseHealthController.AddHealth(heal.healingAmount, "red");
                else if (team == "Blue")
                    baseHealthController.AddHealth(heal.healingAmount, "blue");

                Debug.Log($"Healing object collected by {team} team. Base healed by {heal.healingAmount}.");
                break;
            }
        }
    }

    private void DamageAnimation(string base_color)
    {
        if (explosion == null)
        {
            Debug.LogError("Explosion prefab not assigned in Combat script!");
            return;
        }

        GameObject fx = CFX_SpawnSystem.GetNextObject(explosion);
        if (fx == null)
        {
            Debug.LogWarning("No available explosion instances in pool!");
            return;
        }

        fx.transform.position = (base_color == "blue") ? BLUEexplosionCordinates : REDexplosionCordinates;
        fx.transform.rotation = Quaternion.identity;
        fx.SetActive(true);
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

// ✅ NEW STRUCT FOR MULTIPLE HEALERS
[System.Serializable]
public class HealingObject
{
    public GameObject prefab;
    public int healingAmount;
}

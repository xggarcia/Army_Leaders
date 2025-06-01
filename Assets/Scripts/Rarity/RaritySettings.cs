using UnityEngine;

[System.Serializable]
public class RaritySettings
{
    public Rarity rarity;
    [Range(0f, 1f)] public float spawnProbability;
    public Color glowColor;
}

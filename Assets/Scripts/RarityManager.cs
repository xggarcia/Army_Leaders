using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rarity/RarityManager")]
public class RarityManager : ScriptableObject
{
    public List<RaritySettings> raritySettings;

    public Rarity GetRandomRarity(bool epicOnly = false)
    {
        float roll = Random.value;
        float cumulative = 0f;

        foreach (var setting in raritySettings)
        {
            if (epicOnly && (setting.rarity == Rarity.Common || setting.rarity == Rarity.Rare))
                continue;

            cumulative += setting.spawnProbability;
            if (roll <= cumulative)
                return setting.rarity;
        }

        return Rarity.Legendary; // fallback
    }

    public Color GetColor(Rarity rarity)
    {
        foreach (var setting in raritySettings)
        {
            if (setting.rarity == rarity)
                return setting.glowColor;
        }

        return Color.white;
    }
}

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatIconEntry
{
    public string statName;   // ¿¹: "attack", "hp", "hungerRecovery"
    public Sprite icon;
}

[CreateAssetMenu(menuName = "UI/Icon Mapping")]
public class StatIconDatabase : ScriptableObject
{
    public List<StatIconEntry> iconEntries;

    private Dictionary<string, Sprite> iconMap;

    public Sprite GetIcon(string statName)
    {
        if (iconMap == null)
        {
            iconMap = new Dictionary<string, Sprite>();
            foreach (var entry in iconEntries)
                iconMap[entry.statName] = entry.icon;
        }

        return iconMap.TryGetValue(statName, out var icon) ? icon : null;
    }
}
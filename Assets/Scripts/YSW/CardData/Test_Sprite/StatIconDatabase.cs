using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StatIconEntry
{
    public string statName;   // ¿¹: "attack", "hp", "hungerRecovery"
    public Sprite icon;
    public string statMappingName;
}

[CreateAssetMenu(menuName = "UI/Icon Mapping")]
public class StatIconDatabase : ScriptableObject
{
    public List<StatIconEntry> iconEntries;

    private Dictionary<string, Sprite> iconMap;
    private Dictionary<string, Text> nameMap;

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

    public Text GetName(string statName)
    {
        if(nameMap == null)
        {
            nameMap = new Dictionary<string, Text>();
            foreach (var entry in iconEntries)
            {
                if (entry.statMappingName != null && entry.icon != null)
                {
                    var textObject = new GameObject(entry.statMappingName);
                    var textComponent = textObject.AddComponent<Text>();
                    textComponent.text = entry.statMappingName;
                    nameMap[entry.statName] = textComponent;
                }
            }
        }
        
        return nameMap.TryGetValue(statName, out var text) ? text : null;
    }
}
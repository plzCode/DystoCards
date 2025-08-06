using System.Collections.Generic;
using UnityEngine;

public class StatPanelManager : MonoBehaviour
{
    private HumanCardData lastData;

    public Transform statContainer; // 스탯 항목들을 담는 부모 오브젝트
    public GameObject statEntryPrefab; // StatUIEntry 프리팹

    public void DisplayStats(HumanCardData data, Human human, Card2D card)
    {
        foreach (Transform child in statContainer)
            Destroy(child.gameObject);
        
        Dictionary<string, float> stats = card.GetStatDictionaryFromCardData(data);

        foreach(var kv in stats)
        {
            if (kv.Key == "size") continue; // size는 제외
            switch (kv)
            {
                case var _ when kv.Key == "hp":
                    CreateEntry(kv.Key, human.humanData.MaxHealth, kv.Value, StatVisualType.Bar);
                    continue;
                case var _ when kv.Key == "sanity":
                    CreateEntry(kv.Key, human.humanData.MaxMentalHealth, kv.Value, StatVisualType.Bar);
                    continue;
                case var _ when kv.Key == "stamina":
                    CreateEntry(kv.Key, human.humanData.Stamina, kv.Value, StatVisualType.Bar);
                    continue;
                case var _ when kv.Key == "hunger":
                    CreateEntry(kv.Key, human.humanData.MaxHunger, kv.Value,  StatVisualType.Bar);
                    continue;
                case var _ when kv.Key == "consumeHunger":
                    CreateEntry(kv.Key, human.humanData.ConsumeHunger, 0, StatVisualType.Number);
                    continue;
                case var _ when kv.Key == "attack":
                    CreateEntry(kv.Key, human.humanData.AttackPower, 0, StatVisualType.Number);
                    continue;
                case var _ when kv.Key == "defense":
                    CreateEntry(kv.Key, human.humanData.DefensePower, 0, StatVisualType.Number);
                    continue;
            }            
        }

        /*CreateEntry("체력", human.currentHealth, data.max_health, StatVisualType.Bar);
        CreateEntry("정신력", human.currentMentalHealth, data.max_mental_health, StatVisualType.Bar);
        CreateEntry("스태미너", human.currentStamina, data.stamina, StatVisualType.Bar);
        CreateEntry("공격력", data.attack_power, 0, StatVisualType.Number);
        CreateEntry("방어력", data.defense_power, 0, StatVisualType.Number);
        CreateEntry("허기", human.currentHunger, data.max_hunger, StatVisualType.Bar);
        CreateEntry("소모 허기", data.consume_hunger, 0, StatVisualType.Number);*/
    }

    private void CreateEntry(string name, float value, float max, StatVisualType type)
    {
        GameObject go = Instantiate(statEntryPrefab, statContainer);
        StatUIEntry entry = go.GetComponent<StatUIEntry>();
        entry.SetStat(name, value, max, type);
        go.name = name;
    }
}
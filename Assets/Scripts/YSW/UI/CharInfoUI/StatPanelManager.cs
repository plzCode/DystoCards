using System.Collections.Generic;
using UnityEngine;

public class StatPanelManager : MonoBehaviour
{
    private HumanCardData lastData;

    public Transform statContainer; // ���� �׸���� ��� �θ� ������Ʈ
    public GameObject statEntryPrefab; // StatUIEntry ������

    public void DisplayStats(HumanCardData data, Human human, Card2D card)
    {
        foreach (Transform child in statContainer)
            Destroy(child.gameObject);
        
        Dictionary<string, float> stats = card.GetStatDictionaryFromCardData(data);

        foreach(var kv in stats)
        {
            if (kv.Key == "size") continue; // size�� ����
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

        /*CreateEntry("ü��", human.currentHealth, data.max_health, StatVisualType.Bar);
        CreateEntry("���ŷ�", human.currentMentalHealth, data.max_mental_health, StatVisualType.Bar);
        CreateEntry("���¹̳�", human.currentStamina, data.stamina, StatVisualType.Bar);
        CreateEntry("���ݷ�", data.attack_power, 0, StatVisualType.Number);
        CreateEntry("����", data.defense_power, 0, StatVisualType.Number);
        CreateEntry("���", human.currentHunger, data.max_hunger, StatVisualType.Bar);
        CreateEntry("�Ҹ� ���", data.consume_hunger, 0, StatVisualType.Number);*/
    }

    private void CreateEntry(string name, float value, float max, StatVisualType type)
    {
        GameObject go = Instantiate(statEntryPrefab, statContainer);
        StatUIEntry entry = go.GetComponent<StatUIEntry>();
        entry.SetStat(name, value, max, type);
        go.name = name;
    }
}
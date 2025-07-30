using System.Collections.Generic;
using UnityEngine;

public class Human : Character
{
    public float currentMentalHealth;
    public float currentHunger;
    public float currentStamina;

    //private HumanCardData _runtimeData;

    [Header("Inspectable Equipment Slots")]
    public List<EquipmentSlotData> equipmentSlotList = new();  // ReorderableList�� ���

    private Dictionary<EquipmentSlot, EquipmentCardData> equippedItems = new();

    public HumanCardData humanData => GetComponent<Card2D>().RuntimeData as HumanCardData;

    public void Start()
    {
        var card = GetComponent<Card2D>();
        if (card != null && card.RuntimeData is HumanCardData data)
        {
            base.charData = data;
            Initialize(data);
        }
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            humanData.AttackPower += 1; 
            Debug.Log(ReferenceEquals(humanData, GetComponent<Card2D>().RuntimeData));
        }
    }

    public void ChangeCharData(HumanCardData data)
    {
        base.charData = data;

        var card = GetComponent<Card2D>();
        if (card != null)
        {
            card.SetRuntimeData(data); // ���ο��� Clone�� �̺�Ʈ ���, UI ���� ó��
            Initialize(data);
        }
    }

    public void Initialize(HumanCardData data)
    {
        //_runtimeData = data;

        currentHealth = data.MaxHealth;
        currentMentalHealth = data.max_mental_health;
        currentHunger = data.max_hunger;
        currentStamina = data.stamina;
    }

    public void ConsumeFood()
    {
        currentHunger = Mathf.Max(0, currentHunger - humanData.consume_hunger);
        Debug.Log($"{charData.cardName} consumed {humanData.consume_hunger} hunger. Remaining: {currentHunger}");
    }

    public void RecoverHunger(float amount)
    {
        currentHunger = Mathf.Min(humanData.max_hunger, currentHunger + amount);
        Debug.Log($"{charData.cardName} recovered {amount} hunger. Current: {currentHunger}/{humanData.max_hunger}");
    }

    public void ConsumeStamina(float amount)
    {
        currentStamina = Mathf.Max(0, currentStamina - amount);
        Debug.Log($"{charData.cardName} consumed {amount} stamina. Remaining: {currentStamina}/{humanData.stamina}");
    }

    public void RecoverStamina(float amount)
    {
        currentStamina = Mathf.Min(humanData.stamina, currentStamina + amount);
    }

    public void TakeStress(float amount)
    {
        currentMentalHealth = Mathf.Max(0, currentMentalHealth - amount);
        Debug.Log($"{charData.cardName} took {amount} stress. Mental: {currentMentalHealth}/{humanData.max_mental_health}");
    }

    public void RecoverMentalHealth(float amount)
    {
        currentMentalHealth = Mathf.Min(humanData.max_mental_health, currentMentalHealth + amount);
        Debug.Log($"{charData.cardName} recovered {amount} mental health. Mental: {currentMentalHealth}/{humanData.max_mental_health}");
    }

    public override void Attack(Character target)
    {
        target.TakeDamage(charData.AttackPower);
        Debug.Log($"{charData.cardName} attacks {target.charData.cardName} for {charData.AttackPower} damage");
    }

    public override void TakeDamage(float amount)
    {
        float effectiveDamage = Mathf.Max(0, amount - charData.DefensePower);
        currentHealth = Mathf.Max(0, currentHealth - effectiveDamage);
        Debug.Log($"{charData.cardName} took {effectiveDamage} damage after armor. HP: {currentHealth}/{charData.MaxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Equip(EquipmentCardData equipment)
    {
        if (equipment == null) return;

        // �̹� ������ ��� �ִٸ� ����
        if (equippedItems.TryGetValue(equipment.slot, out var oldEquip))
        {
            RemoveEquipmentStats(oldEquip);
            Debug.Log($"{charData.cardName} unequipped {oldEquip.cardName} from {equipment.slot}");
        }

        equippedItems[equipment.slot] = equipment;
        ApplyEquipmentStats(equipment);

        Debug.Log($"{charData.cardName} equipped {equipment.cardName} on {equipment.slot} = {charData.AttackPower}");
        SyncDictFromList();
    }

    public void Unequip(EquipmentSlot slot)
    {
        if (equippedItems.TryGetValue(slot, out var equip))
        {
            RemoveEquipmentStats(equip);
            equippedItems.Remove(slot);
            Debug.Log($"{charData.cardName} unequipped {equip.cardName} from {slot}");
        }
        SyncDictFromList();
    }

    private void ApplyEquipmentStats(EquipmentCardData equipment)
    {
        if (humanData != null)
        {
            humanData.AttackPower += equipment.attackPower;
            humanData.DefensePower += equipment.defensePower;
            Debug.Log($"{humanData.cardName} gained {equipment.attackPower} ATK from {equipment.cardName}");
        }
    }

    private void RemoveEquipmentStats(EquipmentCardData equipment)
    {
        if (humanData != null)
        {
            humanData.AttackPower -= equipment.attackPower;
            humanData.DefensePower -= equipment.defensePower;
            Debug.Log($"{humanData.cardName} lost {equipment.attackPower} ATK from {equipment.cardName}");
        }
    }

    /*private void ApplyEquipmentStats(EquipmentCardData equipment)
    {
        charData.AttackPower += equipment.attackPower;
        charData.DefensePower += equipment.defensePower;
        Debug.Log($"{charData.cardName} gained {equipment.attackPower} attack and {equipment.defensePower} defense from {equipment.cardName}");
    }

    private void RemoveEquipmentStats(EquipmentCardData equipment)
    {
        charData.AttackPower -= equipment.attackPower;
        charData.DefensePower -= equipment.defensePower;
        Debug.Log($"{charData.cardName} lost {equipment.attackPower} attack and {equipment.defensePower} defense from {equipment.cardName}");
    }*/

    private void SyncDictFromList()
    {
        equipmentSlotList.Clear();
        foreach (var kv in equippedItems)
        {
            equipmentSlotList.Add(new EquipmentSlotData
            {
                slot = kv.Key,
                equipment = kv.Value
            });
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this); // ���� ���� ����
#endif
    }
}

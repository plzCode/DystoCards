using System.Collections.Generic;
using UnityEngine;

public class Human : Character
{
    public float currentMentalHealth;
    public float currentHunger;
    public float currentStamina;

    private HumanCardData _runtimeData;

    [Header("Inspectable Equipment Slots")]
    public List<EquipmentSlotData> equipmentSlotList = new();  // ReorderableList에 사용

    private Dictionary<EquipmentSlot, EquipmentCardData> equippedItems = new();

    public new HumanCardData charData => _runtimeData;

    public void Start()
    {
        if (base.charData != null)
        {
            _runtimeData = Instantiate(base.charData as HumanCardData); // 복제본 생성
            Initialize(_runtimeData);
        }
    }

    public void Initialize(HumanCardData data)
    {
        _runtimeData = data;

        currentHealth = data.max_health;
        currentMentalHealth = data.max_mental_health;
        currentHunger = data.max_hunger;
        currentStamina = data.stamina;
    }

    public void ConsumeFood()
    {
        currentHunger = Mathf.Max(0, currentHunger - charData.consume_hunger);
        Debug.Log($"{charData.cardName} consumed {charData.consume_hunger} hunger. Remaining: {currentHunger}");
    }

    public void RecoverHunger(float amount)
    {
        currentHunger = Mathf.Min(charData.max_hunger, currentHunger + amount);
        Debug.Log($"{charData.cardName} recovered {amount} hunger. Current: {currentHunger}/{charData.max_hunger}");
    }

    public void ConsumeStamina(float amount)
    {
        currentStamina = Mathf.Max(0, currentStamina - amount);
        Debug.Log($"{charData.cardName} consumed {amount} stamina. Remaining: {currentStamina}/{charData.stamina}");
    }

    public void RecoverStamina(float amount)
    {
        currentStamina = Mathf.Min(charData.stamina, currentStamina + amount);
    }

    public void TakeStress(float amount)
    {
        currentMentalHealth = Mathf.Max(0, currentMentalHealth - amount);
        Debug.Log($"{charData.cardName} took {amount} stress. Mental: {currentMentalHealth}/{charData.max_mental_health}");
    }

    public void RecoverMentalHealth(float amount)
    {
        currentMentalHealth = Mathf.Min(charData.max_mental_health, currentMentalHealth + amount);
        Debug.Log($"{charData.cardName} recovered {amount} mental health. Mental: {currentMentalHealth}/{charData.max_mental_health}");
    }

    public override void Attack(Character target)
    {
        target.TakeDamage(charData.attack_power);
        Debug.Log($"{charData.cardName} attacks {target.charData.cardName} for {charData.attack_power} damage");
    }

    public override void TakeDamage(float amount)
    {
        float effectiveDamage = Mathf.Max(0, amount - charData.defense_power);
        currentHealth = Mathf.Max(0, currentHealth - effectiveDamage);
        Debug.Log($"{charData.cardName} took {effectiveDamage} damage after armor. HP: {currentHealth}/{charData.max_health}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Equip(EquipmentCardData equipment)
    {
        if (equipment == null) return;

        // 이미 장착한 장비가 있다면 제거
        if (equippedItems.TryGetValue(equipment.slot, out var oldEquip))
        {
            RemoveEquipmentStats(oldEquip);
            Debug.Log($"{charData.cardName} unequipped {oldEquip.cardName} from {equipment.slot}");
        }

        equippedItems[equipment.slot] = equipment;
        ApplyEquipmentStats(equipment);

        Debug.Log($"{charData.cardName} equipped {equipment.cardName} on {equipment.slot} = {charData.attack_power}");
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
        charData.attack_power += equipment.attackPower;
        charData.defense_power += equipment.defensePower;
        Debug.Log($"{charData.cardName} gained {equipment.attackPower} attack and {equipment.defensePower} defense from {equipment.cardName}");
    }

    private void RemoveEquipmentStats(EquipmentCardData equipment)
    {
        charData.attack_power -= equipment.attackPower;
        charData.defense_power -= equipment.defensePower;
        Debug.Log($"{charData.cardName} lost {equipment.attackPower} attack and {equipment.defensePower} defense from {equipment.cardName}");
    }
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
        UnityEditor.EditorUtility.SetDirty(this); // 변경 사항 저장
#endif
    }
}

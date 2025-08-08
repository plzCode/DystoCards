using System.Collections.Generic;
using UnityEngine;

public class Human : Character
{
    public float currentMentalHealth;
    public float currentHunger;
    public float currentStamina;

    //private HumanCardData _runtimeData;

    [Header("Inspectable Equipment Slots")]
    public List<EquipmentSlotData> equipmentSlotList = new();  // ReorderableList에 사용

    private Dictionary<EquipmentSlot, EquipmentCardData> equippedItems = new();
    private Dictionary<EquipmentSlot, GameObject> equippedObjects = new(); // 장비 오브젝트 저장용

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
        if(Input.GetKeyDown(KeyCode.R))
        {
            humanData.MaxHunger += 1f;
        }
    }

    public void ChangeCharData(HumanCardData data)
    {
        base.charData = data;

        var card = GetComponent<Card2D>();
        if (card != null)
        {
            card.SetRuntimeData(data); // 내부에서 Clone과 이벤트 등록, UI 갱신 처리
            Initialize(data);
        }
    }

    public void Initialize(HumanCardData data)
    {
        //_runtimeData = data;

        currentHealth = data.MaxHealth;
        currentMentalHealth = data.MaxMentalHealth;
        currentHunger = data.MaxHunger;
        currentStamina = data.Stamina;
    }

    public void ConsumeFood()
    {
        currentHunger = Mathf.Max(0, currentHunger - humanData.ConsumeHunger);
        Debug.Log($"{charData.cardName} consumed {humanData.ConsumeHunger} hunger. Remaining: {currentHunger}");
    }

    public void RecoverHunger(float amount)
    {
        currentHunger = Mathf.Min(humanData.MaxHunger, currentHunger + amount);
        Debug.Log($"{charData.cardName} recovered {amount} hunger. Current: {currentHunger}/{humanData.MaxHunger}");
    }

    public void ConsumeStamina(float amount)
    {
        currentStamina = Mathf.Max(0, currentStamina - amount);
        Debug.Log($"{charData.cardName} consumed {amount} stamina. Remaining: {currentStamina}/{humanData.Stamina}");
    }

    public void RecoverStamina(float amount)
    {
        currentStamina = Mathf.Min(humanData.Stamina, currentStamina + amount);
    }

    public void TakeStress(float amount)
    {
        currentMentalHealth = Mathf.Max(0, currentMentalHealth - amount);
        Debug.Log($"{charData.cardName} took {amount} stress. Mental: {currentMentalHealth}/{humanData.MaxMentalHealth}");
    }

    public void RecoverMentalHealth(float amount)
    {
        currentMentalHealth = Mathf.Min(humanData.MaxMentalHealth, currentMentalHealth + amount);
        Debug.Log($"{charData.cardName} recovered {amount} mental health. Mental: {currentMentalHealth}/{humanData.MaxMentalHealth}");
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

    /*public void Equip(EquipmentCardData equipment)
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

        Debug.Log($"{charData.cardName} equipped {equipment.cardName} on {equipment.slot} = {charData.AttackPower}");
        SyncDictFromList();
    }*/
    public void Equip(EquipmentCardData equipment, GameObject itemObj)
    {
        if (equipment == null || itemObj == null) return;

        var slot= equipment.slot;

        if (equippedItems.ContainsKey(slot))
        {
            Unequip(slot);
        }
        
        ApplyEquipmentStats(equipment); 
        equippedItems[equipment.slot] = equipment;
        equippedObjects[slot] = itemObj;

        itemObj.SetActive(false);

        Debug.Log($"{charData.cardName} equipped {equipment.cardName} on {equipment.slot} = {charData.AttackPower}");
        SyncDictFromList();
    }

    /*public void Unequip(EquipmentSlot slot)
    {
        if (equippedItems.TryGetValue(slot, out var equip))
        {
            RemoveEquipmentStats(equip);
            equippedItems.Remove(slot);
            Debug.Log($"{charData.cardName} unequipped {equip.cardName} from {slot}");
        }
        SyncDictFromList();
    }*/

    public void Unequip(EquipmentSlot slot)
    {
        if (!equippedItems.ContainsKey(slot))
        {
            Debug.LogWarning($"[Human] Unequip: {slot} 슬롯에 장착된 장비가 없습니다.");
            return;
        }

        EquipmentCardData itemData = equippedItems[slot];
        GameObject itemObject = equippedObjects[slot];

        // 스탯 제거
        RemoveEquipmentStats(itemData);

        // 장비 오브젝트 활성화 및 위치 복원
        itemObject.SetActive(true); 
        
        var card2D = itemObject.GetComponent<Card2D>();
        card2D.BringToFrontRecursive(card2D);

        Vector3 directionToCenter = (Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, Camera.main.nearClipPlane)) - transform.position).normalized;
        StartCoroutine(card2D.MoveItemLerp(itemObject.transform, transform.position + directionToCenter * 3.0f, 0.6f)); // 3f는 거리
        card2D.cardAnim.PlayFeedBack_ByName("BounceY");
        itemObject.transform.SetParent(CardManager.Instance.cardParent); // 부모에서 분리

        // 슬롯 정보 제거
        equippedItems.Remove(slot);
        equippedObjects.Remove(slot);

        Debug.Log($"[Human] {slot} 슬롯의 {itemData.cardName} 장비를 해제했습니다.");
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
        UnityEditor.EditorUtility.SetDirty(this); // 변경 사항 저장
#endif
    }
}

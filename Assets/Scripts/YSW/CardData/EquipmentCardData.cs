using UnityEngine;

[CreateAssetMenu(menuName = "Cards/03.EquipmentCard", order = 4)]
public class EquipmentCardData : CardData
{
    public int attackPower;
    public int defensePower;
    public EquipmentSlot slot;

    public override CardData Clone()
    {
        EquipmentCardData clone = ScriptableObject.CreateInstance<EquipmentCardData>();

        // 부모 클래스 필드 복사
        clone.cardId = this.cardId;
        clone.cardName = this.cardName;
        clone.cardImage = this.cardImage;
        clone.cardType = this.cardType;
        clone.description = this.description;
        clone.size = this.size;

        clone.attackPower = this.attackPower;
        clone.defensePower = this.defensePower;
        clone.slot = this.slot;

        return clone;
    }
}

public enum EquipmentSlot
{
    Head,
    Body,
    Hand,
}

using UnityEngine;

[CreateAssetMenu(menuName = "Cards/03.EquipmentCard", order = 4)]
public class EquipmentCardData : CardData
{
    public int attackPower;
    public int defensePower;
    public EquipmentSlot slot;
}

public enum EquipmentSlot
{
    Head,
    Body,
    Hand,
}

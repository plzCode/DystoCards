using UnityEngine;

[CreateAssetMenu(menuName = "Cards/EquipmentCard")]
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
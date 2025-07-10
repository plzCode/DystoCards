using UnityEngine;

[CreateAssetMenu(menuName = "Cards/05.FurnitureCard", order = 6)]
public class FurnitureCardData : CardData
{
    public FurnitureType furnitureType;
}
public enum  FurnitureType
{
    Recovery,
    //... 추후 추가
    
}
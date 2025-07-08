using UnityEngine;

public enum CardType { Resource, Character, Equipment, Food, Furniture, Recipe, Event }

[CreateAssetMenu(fileName = "NewCardData", menuName = "Cards/Card Data", order = 1)]
public class CardData : ScriptableObject
{
    public string cardId;
    public string cardName;
    public CardType cardType;
    public string description;
}

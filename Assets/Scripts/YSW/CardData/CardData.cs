using UnityEngine;

public enum CardType {
    Resource,
    Food,
    Equipment,    
    Heal,
    Furniture,
    Recipe,
    Character,
    Event,
}

[CreateAssetMenu(fileName = "NewCardData", menuName = "Cards/Card Data", order = 1)]
public class CardData : ScriptableObject
{
    public string cardId;
    public string cardName;
    public Sprite cardImage;
    public CardType cardType;
    public string description;

    public CardType GetCardType()
    {
        return cardType;
    }
}

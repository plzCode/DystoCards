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
    public int size = 1; // 카드 차지량

    public CardType GetCardType()
    {
        return cardType;
    }
}

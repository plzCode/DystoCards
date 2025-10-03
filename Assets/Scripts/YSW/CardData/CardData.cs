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
    Tech,
    Facility,
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

    public virtual CardData Clone()
    {
        CardData clone = ScriptableObject.CreateInstance<CardData>();
        clone.cardId = this.cardId;
        clone.cardName = this.cardName;
        clone.cardImage = this.cardImage;
        clone.cardType = this.cardType;
        clone.description = this.description;
        clone.size = this.size;
        return clone;
    }
}

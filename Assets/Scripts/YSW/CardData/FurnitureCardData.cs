using UnityEngine;

[CreateAssetMenu(menuName = "Cards/05.FurnitureCard", order = 6)]
public class FurnitureCardData : CardData
{
    public FurnitureType furnitureType;

    public override CardData Clone()
    {
        FurnitureCardData clone = ScriptableObject.CreateInstance<FurnitureCardData>();

        // �θ� Ŭ���� �ʵ� ����
        clone.cardId = this.cardId;
        clone.cardName = this.cardName;
        clone.cardImage = this.cardImage;
        clone.cardType = this.cardType;
        clone.description = this.description;
        clone.size = this.size;

        clone.furnitureType = this.furnitureType;

        return clone;
    }
}
public enum  FurnitureType
{
    Recovery,
    //... ���� �߰�
    
}
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/01.ResourceCard", order = 2)]
public class ResourceCardData : CardData
{
    public int quantity;          // �ڿ� ����
    public ResourceType resourceType;

    public override CardData Clone()
    {
        ResourceCardData clone = ScriptableObject.CreateInstance<ResourceCardData>();

        // �θ� Ŭ���� �ʵ� ����
        clone.cardId = this.cardId;
        clone.cardName = this.cardName;
        clone.cardImage = this.cardImage;
        clone.cardType = this.cardType;
        clone.description = this.description;
        clone.size = this.size;

        clone.quantity = this.quantity;
        clone.resourceType = this.resourceType;

        return clone;
    }
}

public enum ResourceType
{
    Wood,
    Stone,
}

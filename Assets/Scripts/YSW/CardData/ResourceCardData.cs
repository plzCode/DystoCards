using UnityEngine;

[CreateAssetMenu(menuName = "Cards/01.ResourceCard", order = 2)]
public class ResourceCardData : CardData
{
    public int quantity;          // �ڿ� ����
    public ResourceType resourceType;
}

public enum ResourceType
{
    Wood,
    Stone,
}

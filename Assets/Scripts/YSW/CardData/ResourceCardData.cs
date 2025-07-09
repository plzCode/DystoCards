using UnityEngine;

[CreateAssetMenu(menuName = "Cards/ResourceCard")]
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

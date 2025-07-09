using UnityEngine;

[CreateAssetMenu(menuName = "Cards/ResourceCard")]
public class ResourceCardData : CardData
{
    public int quantity;          // 자원 수량
    public ResourceType resourceType;
}

public enum ResourceType
{
    Wood,
    Stone,
}

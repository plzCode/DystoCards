using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLocationInfo", menuName = "Game/Location Info")]
public class LocationInfo : ScriptableObject
{
    [Header("기본 정보")]
    public string locationName;
    public Sprite locationImage;
    [Range(0, 10)] public int requiredStrength;
    [Range(0, 10)] public int requiredStamina;
    [Range(0, 10)] public int dangerLevel;
    [Range(0, 10)] public int durationDays;
    

    [Header("보상 목록")]
    public List<RewardInfo> rewards;
}


[System.Serializable]
public class RewardInfo
{
    public CardData card;     // EquipmentCardData 포함한 모든 카드 참조 가능
    public int quantity;
}
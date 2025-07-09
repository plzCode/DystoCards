using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLocationInfo", menuName = "Game/Location Info")]
public class LocationInfo : ScriptableObject
{
    [Header("기본 정보")]
    public string locationName;
    [Range(0, 10)] public int requiredStrength;
    [Range(0, 10)] public int requiredStamina;
    [Range(0, 10)] public int dangerLevel;

    [Header("보상 목록")]
    public List<RewardInfo> rewards;
}


[System.Serializable]
public class RewardInfo
{
    public string rewardName;
    public Sprite rewardIcon;
    public int quantity;
}
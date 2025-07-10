using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLocationInfo", menuName = "Game/Location Info")]
public class LocationInfo : ScriptableObject
{
    [Header("�⺻ ����")]
    public string locationName;
    public Sprite locationImage;
    [Range(0, 10)] public int requiredStrength;
    [Range(0, 10)] public int requiredStamina;
    [Range(0, 10)] public int dangerLevel;
    [Range(0, 10)] public int durationDays;
    

    [Header("���� ���")]
    public List<RewardInfo> rewards;
}


[System.Serializable]
public class RewardInfo
{
    public CardData card;     // EquipmentCardData ������ ��� ī�� ���� ����
    public int quantity;
}
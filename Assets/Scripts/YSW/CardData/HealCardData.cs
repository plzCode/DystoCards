using UnityEngine;

[CreateAssetMenu(menuName = "Cards/04.HealCard", order = 5)]
public class HealCardData : CardData
{
    public int healthAmount;     // 체력 회복량
    public int mentalAmount;     // 정신력 회복량
    public int maxReuseNum;       // 여러 번 사용 가능한가?
    public int currentReuseNum; // 현재 사용 횟수
}
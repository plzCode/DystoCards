using UnityEngine;

[CreateAssetMenu(menuName = "Cards/HealCard")]
public class HealCardData : CardData
{
    public int healthRestore;     // 체력 회복량
    public int mentalRestore;     // 정신력 회복량
    public bool canTargetOthers;  // 다른 캐릭터에게 사용 가능한가?
    public bool isReusable;       // 여러 번 사용 가능한가?
}
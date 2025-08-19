using UnityEngine;

[CreateAssetMenu(menuName = "Cards/04.HealCard", order = 5)]
public class HealCardData : CardData
{
    public int healthAmount;     // 체력 회복량
    public int mentalAmount;     // 정신력 회복량
    public int staninaAmount;     // 스태미나 회복량
    public int maxReuseNum;       // 여러 번 사용 가능한가?
    public int currentReuseNum; // 현재 사용 횟수
    public string audioRef;

    public override CardData Clone()
    {
        HealCardData clone = ScriptableObject.CreateInstance<HealCardData>();

        // 부모 클래스 필드 복사
        clone.cardId = this.cardId;
        clone.cardName = this.cardName;
        clone.cardImage = this.cardImage;
        clone.cardType = this.cardType;
        clone.description = this.description;
        clone.size = this.size;

        clone.healthAmount = this.healthAmount;
        clone.mentalAmount = this.mentalAmount;
        clone.staninaAmount = this.staninaAmount;
        clone.maxReuseNum = this.maxReuseNum;
        clone.currentReuseNum = this.currentReuseNum;
        clone.audioRef = this.audioRef;

        return clone;
    }
}
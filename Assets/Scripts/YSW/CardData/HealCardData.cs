using UnityEngine;

[CreateAssetMenu(menuName = "Cards/04.HealCard", order = 5)]
public class HealCardData : CardData
{
    public int healthAmount;     // ü�� ȸ����
    public int mentalAmount;     // ���ŷ� ȸ����
    public int staninaAmount;     // ���¹̳� ȸ����
    public int maxReuseNum;       // ���� �� ��� �����Ѱ�?
    public int currentReuseNum; // ���� ��� Ƚ��
    public string audioRef;

    public override CardData Clone()
    {
        HealCardData clone = ScriptableObject.CreateInstance<HealCardData>();

        // �θ� Ŭ���� �ʵ� ����
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
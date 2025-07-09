using UnityEngine;

[CreateAssetMenu(menuName = "Cards/04.HealCard", order = 5)]
public class HealCardData : CardData
{
    public int healthAmount;     // ü�� ȸ����
    public int mentalAmount;     // ���ŷ� ȸ����
    public int maxReuseNum;       // ���� �� ��� �����Ѱ�?
    public int currentReuseNum; // ���� ��� Ƚ��
}
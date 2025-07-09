using UnityEngine;

[CreateAssetMenu(menuName = "Cards/HealCard")]
public class HealCardData : CardData
{
    public int healthRestore;     // ü�� ȸ����
    public int mentalRestore;     // ���ŷ� ȸ����
    public bool canTargetOthers;  // �ٸ� ĳ���Ϳ��� ��� �����Ѱ�?
    public bool isReusable;       // ���� �� ��� �����Ѱ�?
}
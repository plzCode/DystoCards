using UnityEngine;

[CreateAssetMenu(menuName = "Cards/FoodCard")]
public class FoodCardData : CardData
{
    public int hungerRestore;        // ��� ȸ����
    public bool isPerishable;        // ���� ����
    public int shelfLifeTurns;       // �� ���� ������ �����ϴ°�
    //public GameObject spoiledVersion; // ���� �� ��ü ī�� (��: RottenFood)
}
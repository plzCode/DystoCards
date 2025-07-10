using UnityEngine;

[CreateAssetMenu(menuName = "Cards/02.FoodCard", order = 3)]
public class FoodCardData : CardData
{
    public int hungerRestore;        // ��� ȸ����
    public bool isPerishable;        // ���� ����
    public int shelfLifeTurns;       // �� ���� ������ �����ϴ°�
    //public GameObject spoiledVersion; // ���� �� ��ü ī�� (��: RottenFood)
}
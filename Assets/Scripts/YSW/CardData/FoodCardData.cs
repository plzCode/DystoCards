using UnityEngine;

[CreateAssetMenu(menuName = "Cards/02.FoodCard", order = 3)]
public class FoodCardData : CardData
{
    public int hungerRestore;        // ��� ȸ����
    public bool isPerishable;        // ���� ����
    public int shelfLifeTurns;       // �� ���� ������ �����ϴ°�
    //public GameObject spoiledVersion; // ���� �� ��ü ī�� (��: RottenFood)

    public override CardData Clone()
    {
        FoodCardData clone = ScriptableObject.CreateInstance<FoodCardData>();

        // �θ� Ŭ���� �ʵ� ����
        clone.cardId = this.cardId;
        clone.cardName = this.cardName;
        clone.cardImage = this.cardImage;
        clone.cardType = this.cardType;
        clone.description = this.description;
        clone.size = this.size;

        clone.hungerRestore = this.hungerRestore;
        clone.isPerishable = this.isPerishable;
        clone.shelfLifeTurns = this.shelfLifeTurns;

        return clone;
    }
}
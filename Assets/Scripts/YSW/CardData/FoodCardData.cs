using UnityEngine;
using UnityEngine.AddressableAssets;        

[CreateAssetMenu(menuName = "Cards/02.FoodCard", order = 3)]
public class FoodCardData : CardData
{
    public int hungerRestore;        // ��� ȸ����
    public bool isPerishable;        // ���� ����
    private int shelfLifeTurns;     // �� ���� ������ �����ϴ°�}

    public string audioRef;
    //public GameObject spoiledVersion; // ���� �� ��ü ī�� (��: RottenFood)

    public event System.Action OnDataChanged;

    public int ShelfLifeTurns
    {
        get => shelfLifeTurns;
        set
        {
            if (shelfLifeTurns != value)
            {
                shelfLifeTurns = value;
                OnDataChanged?.Invoke();
            }
        }
    }


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
        clone.audioRef = this.audioRef;
        return clone;
    }


}
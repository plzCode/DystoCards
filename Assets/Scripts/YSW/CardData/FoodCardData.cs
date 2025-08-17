using UnityEngine;
using UnityEngine.AddressableAssets;        

[CreateAssetMenu(menuName = "Cards/02.FoodCard", order = 3)]
public class FoodCardData : CardData
{
    public int hungerRestore;        // 허기 회복량
    public bool isPerishable;        // 부패 여부
    private int shelfLifeTurns;     // 몇 턴이 지나면 부패하는가}

    public string audioRef;
    //public GameObject spoiledVersion; // 부패 후 대체 카드 (예: RottenFood)

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
        
        // 부모 클래스 필드 복사
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
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/FoodCard")]
public class FoodCardData : CardData
{
    public int hungerRestore;        // 허기 회복량
    public bool isPerishable;        // 부패 여부
    public int shelfLifeTurns;       // 몇 턴이 지나면 부패하는가
    //public GameObject spoiledVersion; // 부패 후 대체 카드 (예: RottenFood)
}
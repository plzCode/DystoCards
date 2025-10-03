using System.Collections.Generic;
using UnityEngine;

public class FoodManager : MonoBehaviour
{

    private void Start()
    {
        TurnManager.Instance.RegisterPhaseAction(TurnPhase.DayEnd, () => ReduceFoodRemainDays());
    }

    void ReduceFoodRemainDays()
    {
        List<Card2D> foodCards2D = CardManager.Instance.GetCardsByType(CardType.Food);
        for(int i = foodCards2D.Count - 1; i >= 0; i--)
        {
            FoodCardData foodCardData = foodCards2D[i].RuntimeData as FoodCardData;
            foodCardData.ShelfLifeTurns--;            
            if (foodCardData.ShelfLifeTurns <= 0)
            {
                CardManager.Instance.DestroyCard(foodCards2D[i]);
            }
        }

        TurnManager.Instance.MarkActionComplete();
    }
}

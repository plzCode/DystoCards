using UnityEngine;

public class FoodCard2D : Card2D
{
    [SerializeField] private FoodCardData foodData => RuntimeData as FoodCardData;

    

    public override void StackOnto(Card2D target)
    {
        base.StackOnto(target);

        if (target.TryGetComponent<Human>(out var human))
        {
            
            if(foodData != null)
            {
                AudioManager.Instance.PlaySFX(foodData.audioRef);
                human.RecoverHunger(foodData.hungerRestore);
                Debug.Log($"Recover Hunger {foodData.cardName} to {human.charData.cardName}");

                // �ڽ� ī�� ���� �и�
                DetachChildrenBeforeDestroy();

                // �θ���� ���ᵵ ����
                if (parentCard != null)
                {
                    parentCard.childCards.Remove(this);
                    parentCard = null;
                }

                target.childCards.Remove(this);

                CardManager.Instance.DestroyCard(this);
            }
        }
    }

    private void CheckIsPerishable()
    {
        foodData.ShelfLifeTurns--;
        if (foodData.ShelfLifeTurns <= 0)
        {
            Debug.Log("��������� �������ϴ�. ");
            foodData.isPerishable = false;
            CardManager.Instance.DestroyCard(this);
        }
        
    }
    
}

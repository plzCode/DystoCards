using System.Collections.Generic;
using UnityEngine;

public class HealCard2D : Card2D
{
    [SerializeField] private HealCardData healData => RuntimeData as HealCardData;


    public override void StackOnto(Card2D target)
    {
        

        if (target.TryGetComponent<Human>(out var human))
        {

            bool recovered = false;

            recovered |= TryRecover(healData.staninaAmount, human.currentStamina, 5, human.RecoverStamina, "Stamina", healData.cardName, human.charData.cardName);
            recovered |= TryRecover(healData.healthAmount, human.currentHealth, human.humanData.MaxHealth, human.Heal, "Health", healData.cardName, human.charData.cardName);
            recovered |= TryRecover(healData.mentalAmount, human.currentMentalHealth, human.humanData.MaxMentalHealth, human.RecoverMentalHealth, "Mental", healData.cardName, human.charData.cardName);

            if (!recovered)
            {
                Debug.Log("모든 수치가 최대치입니다. 카드 사용을 취소합니다.");
                return; // 아무 것도 회복하지 않았으면 카드 제거 X
            }

            

            // 자식 카드 먼저 분리
            DetachChildrenBeforeDestroy();

            // 부모와의 연결도 정리
            if (parentCard != null)
            {
                parentCard.childCards.Remove(this);
                parentCard = null;
            }

            target.childCards.Remove(this);

            CardManager.Instance.DestroyCard(this);
        }
        else
        {
            base.StackOnto(target);
        }
    }


    //입력 데이터가 0 이상이면 Action 함수를 실행합니다.
    private bool TryRecover(float amount, float current, float max, System.Action<float> recoverAction, string label, string cardName, string targetName)
    {
        if (amount <= 0f) return false;
        if (current >= max) return false;

        recoverAction.Invoke(amount);
        Debug.Log($"Recover {label} {cardName} to {targetName}");
        return true;
    }
        

    
}

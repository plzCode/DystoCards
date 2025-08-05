using System.Collections.Generic;
using UnityEngine;

public class HealCard2D : Card2D
{
    [SerializeField] private HealCardData healData => RuntimeData as HealCardData;


    public override void StackOnto(Card2D target)
    {
        base.StackOnto(target);

        if (target.TryGetComponent<Human>(out var human))
        {

            TryRecover(healData.staninaAmount, (int x) => human.RecoverStamina(x), "Stamina", healData.cardName, human.charData.cardName);
            TryRecover(healData.healthAmount, (int x) => human.TakeDamage(-x), "Health", healData.cardName, human.charData.cardName);
            TryRecover((float)healData.mentalAmount, human.RecoverMentalHealth, "MentalHealth", healData.cardName, human.charData.cardName);

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
    }
    

    //입력 데이터가 0 이상이면 Action 함수를 실행합니다.
    private void TryRecover<T>(T amount, System.Action<T> recoverAction, string label, string cardName, string targetName)
    {
        if (Comparer<T>.Default.Compare(amount, default) > 0)
        {
            recoverAction.Invoke(amount);
            Debug.Log($"Recover {label} {cardName} to {targetName}");
        }
    }
}

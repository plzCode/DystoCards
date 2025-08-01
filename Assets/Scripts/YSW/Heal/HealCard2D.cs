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
    

    //�Է� �����Ͱ� 0 �̻��̸� Action �Լ��� �����մϴ�.
    private void TryRecover<T>(T amount, System.Action<T> recoverAction, string label, string cardName, string targetName)
    {
        if (Comparer<T>.Default.Compare(amount, default) > 0)
        {
            recoverAction.Invoke(amount);
            Debug.Log($"Recover {label} {cardName} to {targetName}");
        }
    }
}

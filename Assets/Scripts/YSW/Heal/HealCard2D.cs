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
                Debug.Log("��� ��ġ�� �ִ�ġ�Դϴ�. ī�� ����� ����մϴ�.");
                return; // �ƹ� �͵� ȸ������ �ʾ����� ī�� ���� X
            }

            

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
        else
        {
            base.StackOnto(target);
        }
    }


    //�Է� �����Ͱ� 0 �̻��̸� Action �Լ��� �����մϴ�.
    private bool TryRecover(float amount, float current, float max, System.Action<float> recoverAction, string label, string cardName, string targetName)
    {
        if (amount <= 0f) return false;
        if (current >= max) return false;

        recoverAction.Invoke(amount);
        Debug.Log($"Recover {label} {cardName} to {targetName}");
        return true;
    }
        

    
}

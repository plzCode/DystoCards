using UnityEngine;

public class EquipmentCard2D : Card2D
{
    public override void StackOnto(Card2D target)
    {
        base.StackOnto(target);

        if (target.TryGetComponent<Human>(out var human))
        {
            var equipData = cardData as EquipmentCardData;
            if (equipData != null)
            {
                human.Equip(equipData, this.gameObject);
                Debug.Log($"Equipped {equipData.cardName} to {human.charData.cardName}");

                // �ڽ� ī�� ���� �и�
                DetachChildrenBeforeDestroy();

                // �θ���� ���ᵵ ����
                if (parentCard != null)
                {
                    parentCard.childCards.Remove(this);
                    parentCard = null;
                }

                target.childCards.Remove(this);

                //Destroy(gameObject);
                gameObject.SetActive(false); // ���� ������Ʈ ��Ȱ��ȭ
            }
        }
    }
}
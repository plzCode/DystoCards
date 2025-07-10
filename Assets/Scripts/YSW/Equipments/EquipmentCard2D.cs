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
                human.Equip(equipData);
                Debug.Log($"Equipped {equipData.cardName} to {human.charData.cardName}");

                // 자식 카드 먼저 분리
                DetachChildrenBeforeDestroy();

                // 부모와의 연결도 정리
                if (parentCard != null)
                {
                    parentCard.childCards.Remove(this);
                    parentCard = null;
                }

                target.childCards.Remove(this);

                Destroy(gameObject);
            }
        }
    }
}
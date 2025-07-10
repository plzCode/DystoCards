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
                Destroy(gameObject);
            }
        }
    }
}
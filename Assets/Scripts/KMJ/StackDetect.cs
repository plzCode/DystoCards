using UnityEngine;
using UnityEngine.EventSystems;

public class StackDetect : MonoBehaviour, IEndDragHandler
{
    public void OnEndDrag(PointerEventData e)
    {
        if (!InputGate.Enabled) return;

        if (transform.parent.TryGetComponent(out Card2D facCard) &&
            facCard.RuntimeData is FacilityCardData f &&
            GetComponent<CharacterCard2D>() is CharacterCard2D chr)
        {
            ActionMenu.Instance.Open(f.facilityType, chr);
        }
    }
}

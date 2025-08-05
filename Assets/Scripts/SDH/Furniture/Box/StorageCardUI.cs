using UnityEngine;
using UnityEngine.EventSystems;

public class StorageCardUI : MonoBehaviour, IPointerClickHandler
{
    public Card2D linkedCard; // 실제 카드 참조
    public Card_Storage box;      // 박스 참조

    private float lastClickTime;
    private const float doubleClickThreshold = 0.3f;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.time - lastClickTime < doubleClickThreshold)
        {
            // 더블클릭 발생
            box.RemoveCard(linkedCard);
        }

        lastClickTime = Time.time;
    }
}
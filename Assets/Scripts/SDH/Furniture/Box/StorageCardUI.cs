using UnityEngine;
using UnityEngine.EventSystems;

public class StorageCardUI : MonoBehaviour, IPointerClickHandler
{
    public Card2D linkedCard; // ���� ī�� ����
    public Card_Storage box;      // �ڽ� ����

    private float lastClickTime;
    private const float doubleClickThreshold = 0.3f;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.time - lastClickTime < doubleClickThreshold)
        {
            // ����Ŭ�� �߻�
            box.RemoveCard(linkedCard);
        }

        lastClickTime = Time.time;
    }
}
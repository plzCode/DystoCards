using UnityEngine;
using UnityEngine.EventSystems;

public class BoxCardUI : MonoBehaviour, IPointerClickHandler
{
    public Card2D linkedCard; // ���� ī�� ����
    public Card_Box box;      // �ڽ� ����

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
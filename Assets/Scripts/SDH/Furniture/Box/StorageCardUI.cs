using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ī�� UI�� ����ϴ� Ŭ����
/// ī�� Ŭ�� �̺�Ʈ�� ó���ϸ�, ����Ŭ�� �� �ش� ī�带 ���� �ڽ����� ����
/// </summary>
public class StorageCardUI : MonoBehaviour, IPointerClickHandler
{
    public Card2D linkedCard; // ���� ī�� ������Ʈ ����
    public Card_Storage box;  // ī�尡 ��� �ִ� �ڽ�(�����) ����

    private float lastClickTime; // ������ Ŭ�� �ð� �����
    private const float doubleClickThreshold = 0.3f; // ����Ŭ�� ���� �Ӱ谪 (��)

    /// <summary>
    /// ���콺�� ī�带 Ŭ������ �� ȣ���
    /// ����Ŭ�� �� �ڽ����� ī�带 ������
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // ����Ŭ���̸� ī�� ����
        if (Time.time - lastClickTime < doubleClickThreshold)
        {
            box.RemoveCard(linkedCard);
        }

        // Ŭ�� �ð� ����
        lastClickTime = Time.time;
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class StopClickThrough : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        // �̺�Ʈ ���� ����
        eventData.Use();
    }
}
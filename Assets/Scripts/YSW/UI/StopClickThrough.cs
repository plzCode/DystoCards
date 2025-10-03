using UnityEngine;
using UnityEngine.EventSystems;

public class StopClickThrough : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        // 이벤트 전파 방지
        eventData.Use();
    }
}
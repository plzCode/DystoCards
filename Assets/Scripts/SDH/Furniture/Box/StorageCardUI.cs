using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 카드 UI를 담당하는 클래스
/// 카드 클릭 이벤트를 처리하며, 더블클릭 시 해당 카드를 저장 박스에서 제거
/// </summary>
public class StorageCardUI : MonoBehaviour, IPointerClickHandler
{
    public Card2D linkedCard; // 실제 카드 오브젝트 참조
    public Card_Storage box;  // 카드가 들어 있는 박스(저장소) 참조

    private float lastClickTime; // 마지막 클릭 시간 저장용
    private const float doubleClickThreshold = 0.3f; // 더블클릭 판정 임계값 (초)

    /// <summary>
    /// 마우스로 카드를 클릭했을 때 호출됨
    /// 더블클릭 시 박스에서 카드를 제거함
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // 더블클릭이면 카드 제거
        if (Time.time - lastClickTime < doubleClickThreshold)
        {
            box.RemoveCard(linkedCard);
        }

        // 클릭 시간 갱신
        lastClickTime = Time.time;
    }
}

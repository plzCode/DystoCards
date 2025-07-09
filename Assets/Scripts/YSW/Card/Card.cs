using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public CardData data;
    public TextMeshProUGUI nameText;

    private Transform originalParent;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private Vector2 dragOffset;

    public virtual void Initialize(CardData _data)
    {
        data = _data;
        nameText.text = data.cardName;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public virtual void OnClick()
    {
        Debug.Log($"Clicked on {data.cardName}");
    }

    public virtual void OnDrop(Card droppedOn) { }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(canvas.transform, true);
        canvasGroup.blocksRaycasts = false;

        // 마우스 위치와 카드 중심의 차이 계산
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out dragOffset
        );

        // 카드 좌표계 기준으로 보정
        dragOffset = rectTransform.localPosition - (Vector3)dragOffset;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
        canvas.transform as RectTransform,
        eventData.position,
        eventData.pressEventCamera,
        out Vector2 localMousePos))
        {
            rectTransform.localPosition = localMousePos + dragOffset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent, true);
        canvasGroup.blocksRaycasts = true;
    }
}

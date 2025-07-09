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

        // ���콺 ��ġ�� ī�� �߽��� ���� ���
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out dragOffset
        );

        // ī�� ��ǥ�� �������� ����
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

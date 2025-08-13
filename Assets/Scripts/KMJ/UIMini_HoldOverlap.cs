using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// Canvas 위에서 도끼(Axe)를 드래그하여 나무(Tree)와 겹친 상태로 일정 시간 유지하면 성공
public class UIMini_HoldOverlap : UIMinigameBase, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Refs")]
    [SerializeField] private RectTransform axe;      // 드래그 타깃
    [SerializeField] private RectTransform tree;     // 목표
    [SerializeField] private RectTransform dragArea; // 보통 Canvas의 root 또는 MinigameRoot
    [SerializeField] private Image progress;  // 선택: 진행도 표시(Image fillAmount)

    [Header("Tuning")]
    [SerializeField] private float holdSeconds = 4f; // 4초
    [SerializeField] private float snapRadius = 40f;  // 겹침 판정 느슨함(픽셀)

    private Camera uiCam;
    private bool dragging = false;

    // 남은 시간(초) — 겹칠 때만 줄어듦
    private float remaining;
    protected override void OnStartGame()
    {
        if (dragArea == null)
            dragArea = transform.parent as RectTransform;

        var canvas = GetComponentInParent<Canvas>();
        uiCam = (canvas && canvas.renderMode != RenderMode.ScreenSpaceOverlay) ? canvas.worldCamera : null;

        remaining = holdSeconds;

        if (progress)
        {
            // 꽉 찬 상태에서 시작
            progress.fillAmount = 1f;
        }

        dragging = false;
        // 시작 SFX 등을 여기서 재생 가능
    }

    void Update()
    {
        if (IsOverlapped(axe, tree, snapRadius))
        {
            remaining -= Time.deltaTime;
            if (progress)
                progress.fillAmount = Mathf.Clamp01(remaining / holdSeconds);

            if (remaining <= 0f)
            {
                // 성공! 실패 케이스는 없음
                Complete(true);
                enabled = false; // 더 이상 Update 돌지 않게
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData) => dragging = true;

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                dragArea, eventData.position, uiCam, out var local))
        {
            axe.anchoredPosition = local;
        }
    }

    public void OnEndDrag(PointerEventData eventData) => dragging = false;

    private bool IsOverlapped(RectTransform a, RectTransform b, float radius)
    {
        if (!a || !b) return false;
        Vector3 ca = a.TransformPoint(a.rect.center);
        Vector3 cb = b.TransformPoint(b.rect.center);
        return Vector2.Distance(ca, cb) <= radius;
    }
}
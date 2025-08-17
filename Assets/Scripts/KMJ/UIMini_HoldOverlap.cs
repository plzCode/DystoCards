using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

/// Canvas 위에서 도끼(Axe)를 드래그하여 나무(Tree)와 겹친 상태로 일정 시간 유지하면 성공
public class UIMini_HoldOverlap : UIMinigameBase, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Refs")]
    [SerializeField] private RectTransform Tool;      // 드래그 타깃
    [SerializeField] private RectTransform Target;     // 목표
    [SerializeField] private RectTransform dragArea; // 보통 Canvas의 root 또는 MinigameRoot
    [SerializeField] private Image progress;  // 선택: 진행도 표시(Image fillAmount)

    [SerializeField] private string sfxAddress = "Felling";
    [SerializeField] AudioSource loopSrc;

    [Header("Tuning")]
    [SerializeField] private float holdSeconds = 4f; // 4초
    [SerializeField] private float snapRadius = 40f;  // 겹침 판정 느슨함(픽셀)
    [SerializeField] bool debugLog = false;

    Camera uiCam;
    float remaining;
    bool dragging;
    bool wasOverlapped;
    bool enteredOnce;                 // 실제로 겹친 적이 있어야 카운트 시작
    AsyncOperationHandle<AudioClip>? sfxHandle;

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
        // 사운드 준비(로드만, 재생은 겹침 때)
        if (!string.IsNullOrEmpty(sfxAddress))
        {
            var handle = Addressables.LoadAssetAsync<AudioClip>(sfxAddress);
            handle.Completed += op =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded && loopSrc)
                {
                    loopSrc.clip = op.Result;
                    loopSrc.loop = true;
                }
            };
            sfxHandle = handle;
        }
        if (loopSrc) loopSrc.Stop();
    }
    void OnDestroy()
    {
        if (sfxHandle.HasValue)
        {
            Addressables.Release(sfxHandle.Value);
            sfxHandle = null;
        }
    }

    void Update()
    {
        bool nowOverlapped = IsOverlapped(Tool, Target, snapRadius);

        // 겹침 진입/이탈 시에만 루프 재생/일시정지
        if (loopSrc && loopSrc.clip)
        {
            if (nowOverlapped && !wasOverlapped) loopSrc.Play();
            if (!nowOverlapped && wasOverlapped) loopSrc.Pause();
        }
        wasOverlapped = nowOverlapped;

        // 실제로 한 번이라도 겹친 이후에만 카운트 다운 시작
        if (nowOverlapped) enteredOnce = true;

        if (enteredOnce && nowOverlapped)
        {
            remaining -= Time.deltaTime;
            if (progress) progress.fillAmount = Mathf.Clamp01(remaining / holdSeconds);

            if (remaining <= 0f)
            {
                if (loopSrc) loopSrc.Stop();
                Complete(true);
                enabled = false;
            }
        }

        if (debugLog && Tool && Target)
        {
            var ca = Tool.TransformPoint(Tool.rect.center);
            var cb = Target.TransformPoint(Target.rect.center);
            var dist = Vector2.Distance(ca, cb);
            Debug.Log($"[Mini] overlapped={nowOverlapped}, enteredOnce={enteredOnce}, dist={dist:F1}");
        }
    }



    public void OnBeginDrag(PointerEventData eventData) => dragging = true;

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                dragArea, eventData.position, uiCam, out var local))
        {
            Tool.anchoredPosition = local;
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
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using MoreMountains.Feedbacks;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Tooltip")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;
    public Vector2 tooltipOffset = new Vector2(20f, -20f);

    [Header("Fade")]
    public CanvasGroup fadeCanvas;
    public float defaultFadeDuration = 1f;

    [Header("Cursor")]
    public Texture2D defaultCursor;
    public Texture2D interactCursor;
    public Texture2D denyCursor;
    public Vector2 cursorHotspot = Vector2.zero;

    [Header("Card Info Panels")]
    public GameObject cardInfoPanel;

    [Header("Icon Database")]
    public StatIconDatabase iconDatabase;

    [Header("ClickCatcher")]
    public ClickCatcher clickCatcher;

    [Header("MMF_Player")]
    public MMF_Player showFeedback;
    public MMF_Player hideFeedback;

    [Header("Raycast Control")]
    public LayerMask defaultInteractableMask;
    public LayerMask defaultCardMask;

    public LayerMask uiOnlyInteractableMask; 
    public LayerMask uiOnlyCardMask;

    [Header("Mouse Input")]
    public MouseInput mouseInput;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Start()
    {
        SetDefaultCursor();
    }

    private void Update()
    {
        // Tooltip 위치를 마우스에 따라 갱신
        if (tooltipPanel.activeSelf)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                tooltipPanel.transform.parent.GetComponent<RectTransform>(),
                Input.mousePosition + (Vector3)tooltipOffset,
                null,
                out pos
            );
            tooltipPanel.GetComponent<RectTransform>().anchoredPosition = pos;
        }
    }

    // ─────────────────────────────
    // ▶ Tooltip 기능
    // ─────────────────────────────
    #region Tooltip
    public void ShowTooltip(string message)
    {
        tooltipText.text = message;
        tooltipPanel.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
    #endregion

    // ─────────────────────────────
    // ▶ 페이드 인/아웃
    // ─────────────────────────────
    #region Fade
    public void FadeIn(float duration = -1f)
    {
        StartCoroutine(FadeRoutine(0f, 1f, duration < 0 ? defaultFadeDuration : duration));
    }

    public void FadeOut(float duration = -1f)
    {
        StartCoroutine(FadeRoutine(1f, 0f, duration < 0 ? defaultFadeDuration : duration));
    }

    private IEnumerator FadeRoutine(float from, float to, float duration)
    {
        if (fadeCanvas == null)
        {
            Debug.LogWarning("Fade CanvasGroup not assigned!");
            yield break;
        }

        fadeCanvas.blocksRaycasts = true;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, time / duration);
            fadeCanvas.alpha = alpha;
            yield return null;
        }

        fadeCanvas.alpha = to;
        fadeCanvas.blocksRaycasts = (to > 0.9f);
    }
    public IEnumerator FadeCanvas(CanvasGroup group, float from, float to, float duration)
    {
        if (group == null)
        {
            Debug.LogWarning("FadeCanvas: 대상 CanvasGroup이 없습니다.");
            yield break;
        }

        group.blocksRaycasts = true;
        group.alpha = from;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, time / duration);
            yield return null;
        }

        group.alpha = to;
        group.blocksRaycasts = (to > 0.9f);
    }
    #endregion

    // ─────────────────────────────
    // ▶ UI 패널 토글
    // ─────────────────────────────

    #region UI Panels
    public void TogglePanel(GameObject panel)
    {
        if (panel == null) return;
        
                                             
        if (!panel.activeSelf)
        {
            // 열기
            panel.SetActive(true);

            if(showFeedback != null)
            {
                MMF_Scale scaleFeedback = showFeedback.GetFeedbackOfType<MMF_Scale>();
                scaleFeedback.AnimateScaleTarget = panel.transform;
                showFeedback.PlayFeedbacks();
            }
           
            // 클릭 캐처 활성화
            clickCatcher.panelToClose = panel;
            clickCatcher.gameObject.SetActive(true);

            // 마우스 입력 차단
            if (mouseInput != null)
                mouseInput.SetInteractionLayers(uiOnlyInteractableMask, uiOnlyCardMask);
        }
        else
        {
            if (hideFeedback != null)
            {
                MMF_Scale scaleFeedback = hideFeedback.GetFeedbackOfType<MMF_Scale>();
                scaleFeedback.AnimateScaleTarget = panel.transform;
                hideFeedback.PlayFeedbacks();

                float delay = hideFeedback.TotalDuration; // MMF_Player에서 재생시간 받아오기 (GetDuration() 참고)
                StartCoroutine(DisableAfter(delay, panel));
            }
            else
            {
                panel.SetActive(false);
            }

            // 클릭 캐처 비활성화
            clickCatcher.panelToClose = null;
            clickCatcher.gameObject.SetActive(false);

            //마우스 입력 활성화
            if (mouseInput != null)
                mouseInput.SetInteractionLayers(defaultInteractableMask, defaultCardMask);
        }
    }    

    public IEnumerator DisableAfter(float delay, GameObject panel)
    {
        yield return new WaitForSeconds(delay);
        //TogglePanel(panel); 
        panel.SetActive(false);
    }

    // 나중에 CanvasGroup을 사용하면 쓸 곳
    public void ShowPanel(CanvasGroup group)
    {
        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;
    }

    public void HidePanel(CanvasGroup group)
    {
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;
    }


    public void ShowPanel(GameObject panel)
    {
        if (panel != null)
            panel.SetActive(true);
    }

    public void HidePanel(GameObject panel)
    {
        if (panel != null)
            TogglePanel(panel);
//            panel.SetActive(false);
    }
    #endregion

    // ─────────────────────────────
    // ▶ 커서 관련 기능
    // ─────────────────────────────
    #region Cursor
    public void SetDefaultCursor()
    {
        Cursor.SetCursor(defaultCursor, cursorHotspot, CursorMode.Auto);
    }

    public void SetInteractCursor()
    {
        Cursor.SetCursor(interactCursor, cursorHotspot, CursorMode.Auto);
    }

    public void SetDenyCursor()
    {
        Cursor.SetCursor(denyCursor, cursorHotspot, CursorMode.Auto);
    }
    #endregion
}

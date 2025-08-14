using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 이벤트 UI를 관리하는 클래스
/// 1장의 카드만 보여주고 O/X 버튼으로 선택하는 방식
/// </summary>
public class EventUIManager : MonoBehaviour
{
    [SerializeField] private GameObject eventChoiceUIPanel; // 이벤트 UI Panel (비활성/활성 전환용)
    [SerializeField] private TMP_Text descriptionText;      // 카드 설명을 보여줄 TMP_Text
    [SerializeField] private Button acceptButton;           // O 버튼
    [SerializeField] private Button rejectButton;           // X 버튼
    [SerializeField] private GameObject eventResultUIPanel; // 이벤트 UI Panel (비활성/활성 전환용)
    [SerializeField] private TMP_Text resultText;           // 카드 결과를 보여줄 TMP_Text
    [SerializeField] private Button closeButton;            // X 버튼

    private EventCardData currentCard; // 현재 카드 데이터

    private void Start()
    {
        // 게임 시작 시 비활성화
        eventChoiceUIPanel.SetActive(false);
        eventResultUIPanel.SetActive(false);

        // 버튼 이벤트는 Start에서 한 번만 연결
        acceptButton.onClick.AddListener(AcceptCard);
        rejectButton.onClick.AddListener(RejectCard);
        rejectButton.onClick.AddListener(TurnManager.Instance.MarkActionComplete);
        closeButton.onClick.AddListener(CloseResult);
        closeButton.onClick.AddListener(TurnManager.Instance.MarkActionComplete);

        TurnManager.Instance.RegisterPhaseAction(TurnPhase.EventDraw, () => OpenEventUI());
    }

    /// <summary>
    /// 이벤트 UI를 열고 카드 1장을 랜덤으로 선택
    /// </summary>
    private void OpenEventUI()
    {
        currentCard = EventFunctionManager.Instance.GetRandomCard();

        descriptionText.text = currentCard.description;

        AudioManager.Instance.PlaySFX("EventActivate");

        // Appear 효과로 나타나게
        var dissolve = eventChoiceUIPanel.GetComponent<UI_Dissolve_Modify>();
        if (dissolve != null)
        {
            StartCoroutine(dissolve.Appear(true, true));
        }
        else
        {
            // Dissolve 스크립트 없으면 그냥 활성화
            UIManager.Instance.TogglePanel(eventChoiceUIPanel);
        }
    }

    /// <summary>
    /// 이벤트 UI 닫기
    /// </summary>
    private void CloseResult()
    {
        EventFunctionManager.Instance.Execute(currentCard.functionKey);

        AudioManager.Instance.PlaySFX("EventActivate");

        // Dissolve 효과로 사라지게
        var dissolve = eventResultUIPanel.GetComponent<UI_Dissolve_Modify>();
        if (dissolve != null)
        {
            StartCoroutine(dissolve.Vanish(true, true));
        }
        else
        {
            // Dissolve 스크립트 없으면 그냥 비활성화
            UIManager.Instance.TogglePanel(eventResultUIPanel);
        }

        Recorder.Instance.RecordEvent(currentCard.eventResult, TurnManager.Instance.TurnCount);
    }

    private void AcceptCard()
    {
        StartCoroutine(AcceptCardRoutine());
    }

    /// <summary>
    /// O 버튼 클릭 시 호출
    /// 카드의 기능 실행 후 UI 닫기
    /// </summary>
    private IEnumerator AcceptCardRoutine()
    {
        AudioManager.Instance.PlaySFX("EventActivate");

        // Dissolve 효과로 사라지게
        var dissolveVanish = eventChoiceUIPanel.GetComponent<UI_Dissolve_Modify>();
        if (dissolveVanish != null)
            yield return StartCoroutine(dissolveVanish.Vanish(true, true));
        else
            UIManager.Instance.TogglePanel(eventChoiceUIPanel);

        // 결과 텍스트 설정
        resultText.text = currentCard.eventResult;

        // 잠깐 대기 (0.5초 예시)
        yield return new WaitForSeconds(0.5f);

        AudioManager.Instance.PlaySFX("EventActivate");

        // Appear 효과로 나타나게
        var dissolveAppear = eventResultUIPanel.GetComponent<UI_Dissolve_Modify>();
        if (dissolveAppear != null)
            yield return StartCoroutine(dissolveAppear.Appear(true, true));
        else
            UIManager.Instance.TogglePanel(eventResultUIPanel);
    }

    /// <summary>
    /// X 버튼 클릭 시 호출
    /// 특별한 로직이 없다면 그냥 닫기
    /// </summary>
    private void RejectCard()
    {
        AudioManager.Instance.PlaySFX("EventActivate");

        // Dissolve 효과로 사라지게
        var dissolve = eventChoiceUIPanel.GetComponent<UI_Dissolve_Modify>();
        if (dissolve != null)
        {
            StartCoroutine(dissolve.Vanish(true, true));
        }
        else
        {
            // Dissolve 스크립트 없으면 그냥 비활성화
            UIManager.Instance.TogglePanel(eventChoiceUIPanel);
        }
    }
}

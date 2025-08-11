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
        closeButton.onClick.AddListener(CloseResult);

        TurnManager.Instance.RegisterPhaseAction(TurnPhase.EventDraw, () => OpenEventUI());
    }

    /// <summary>
    /// 이벤트 UI를 열고 카드 1장을 랜덤으로 선택
    /// </summary>
    private void OpenEventUI()
    {
        currentCard = EventFunctionManager.Instance.GetRandomCard();

        descriptionText.text = currentCard.description;

        UIManager.Instance.TogglePanel(eventChoiceUIPanel);

        Debug.Log($"[이벤트] {currentCard.cardName} 등장");
    }

    /// <summary>
    /// 이벤트 UI 닫기
    /// </summary>
    private void CloseResult()
    {
        UIManager.Instance.TogglePanel(eventResultUIPanel);
        EventFunctionManager.Instance.Execute(currentCard.functionKey);
        TurnManager.Instance.MarkActionComplete();
    }

    /// <summary>
    /// O 버튼 클릭 시 호출
    /// 카드의 기능 실행 후 UI 닫기
    /// </summary>
    private void AcceptCard()
    {
        Debug.Log($"[{currentCard.cardName}] O 버튼 선택됨");

        UIManager.Instance.TogglePanel(eventChoiceUIPanel);
        resultText.text = currentCard.eventResult;
        UIManager.Instance.TogglePanel(eventResultUIPanel);
    }

    /// <summary>
    /// X 버튼 클릭 시 호출
    /// 특별한 로직이 없다면 그냥 닫기
    /// </summary>
    private void RejectCard()
    {
        Debug.Log($"[{currentCard.cardName}] X 버튼 선택됨");

        UIManager.Instance.TogglePanel(eventChoiceUIPanel);
    }
}

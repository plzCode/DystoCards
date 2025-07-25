using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 이벤트 UI를 관리하는 클래스
/// 1장의 카드만 보여주고 O/X 버튼으로 선택하는 방식
/// </summary>
public class EventUIManager : MonoBehaviour
{
    [SerializeField] private GameObject eventCanvas; // 이벤트 UI Canvas (비활성/활성 전환용)
    [SerializeField] private TMP_Text cardText;      // 카드 이름을 보여줄 TMP_Text
    [SerializeField] private Button acceptButton;    // O 버튼
    [SerializeField] private Button rejectButton;    // X 버튼

    private EventCardData currentCard; // 현재 카드 데이터

    private void Start()
    {
        eventCanvas.SetActive(false); // 게임 시작 시 비활성화

        // 버튼 이벤트는 Start에서 한 번만 연결
        acceptButton.onClick.AddListener(AcceptCard);
        rejectButton.onClick.AddListener(RejectCard);
    }

    private void Update()
    {
        // 테스트용: T 키를 누르면 이벤트 UI 열기
        if (Input.GetKeyDown(KeyCode.T))
        {
            OpenEventUI();
        }
    }

    /// <summary>
    /// 이벤트 UI를 열고 카드 1장을 랜덤으로 선택
    /// </summary>
    private void OpenEventUI()
    {
        currentCard = EventFunctionManager.Instance.GetRandomCard();

        cardText.text = currentCard.description;

        eventCanvas.SetActive(true);

        Debug.Log($"[이벤트] {currentCard.cardName} 등장");
    }

    /// <summary>
    /// 이벤트 UI 닫기
    /// </summary>
    private void CloseEventUI()
    {
        eventCanvas.SetActive(false);
    }

    /// <summary>
    /// O 버튼 클릭 시 호출
    /// 카드의 기능 실행 후 UI 닫기
    /// </summary>
    private void AcceptCard()
    {
        Debug.Log($"[{currentCard.cardName}] O 버튼 선택됨");

        EventFunctionManager.Instance.Execute(currentCard.functionKey);
        CloseEventUI();
    }

    /// <summary>
    /// X 버튼 클릭 시 호출
    /// 특별한 로직이 없다면 그냥 닫기
    /// </summary>
    private void RejectCard()
    {
        Debug.Log($"[{currentCard.cardName}] X 버튼 선택됨");

        CloseEventUI();
    }
}

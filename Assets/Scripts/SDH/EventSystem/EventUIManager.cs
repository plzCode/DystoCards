using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 이벤트 UI를 관리하는 클래스
/// 카드 선택 이벤트를 처리하며, EventFunctionManager와 연동
/// </summary>
public class EventUIManager : MonoBehaviour
{
    [SerializeField] private GameObject eventCanvas;      // 이벤트 카드 UI용 Canvas (비활성/활성 전환용)
    [SerializeField] private GameObject cardButtonPrefab; // 카드 버튼 프리팹 (버튼 + TMP_Text 포함)
    [SerializeField] private Transform buttonParent;      // 버튼들이 생성될 부모 오브젝트 (Grid Layout Group 사용 권장)

    private List<GameObject> spawnedButtons = new List<GameObject>(); // 생성된 버튼들을 관리하는 리스트
    private EventCardData[] currentCards;                             // 현재 보여줄 카드 데이터 배열
    private int cardCount = 3;                                        // 한 번에 뽑을 카드 개수 (기본값 3개)

    private void Start()
    {
        eventCanvas.SetActive(false); // 게임 시작 시 이벤트 UI를 비활성화
    }

    private void Update()
    {
        // 테스트용: T 키를 누르면 이벤트 UI를 열고 랜덤 카드 생성
        if (Input.GetKeyDown(KeyCode.T))
        {
            currentCards = new EventCardData[0]; // 빈 배열로 초기화 (혹시 모를 이전 데이터 방지)
            OpenEventUI();
        }
    }

    /// <summary>
    /// 이벤트 카드 UI를 열고 랜덤 카드를 받아와 버튼을 생성
    /// </summary>
    private void OpenEventUI()
    {
        // 기존에 생성된 버튼들을 삭제 (중복 생성 방지)
        foreach (var btn in spawnedButtons)
            Destroy(btn);
        
        spawnedButtons.Clear(); // 리스트도 초기화

        cardCount = 3; // 이번 턴 카드 개수 (임의 지정, 필요 시 랜덤화 가능)
        currentCards = EventFunctionManager.Instance.GetRandomCards(cardCount); // 랜덤 카드 가져오기

        eventCanvas.SetActive(true); // 이벤트 카드 UI 활성화

        // 카드 버튼을 생성 및 이벤트 등록
        for (int i = 0; i < currentCards.Length; i++)
        {
            // 버튼 프리팹을 복제해서 부모 오브젝트에 생성 (GridLayoutGroup이 위치를 자동 정렬)
            GameObject buttonObj = Instantiate(cardButtonPrefab, buttonParent);
            spawnedButtons.Add(buttonObj); // 리스트에 추가

            Button button = buttonObj.GetComponent<Button>(); // 버튼 컴포넌트 가져오기

            int index = i; // 클로저 문제 방지용 지역 변수 (람다 안에서 사용)
            button.onClick.AddListener(() => SelectCard(index)); // 버튼 클릭 시 SelectCard 호출

            // 버튼 안에 있는 TMP_Text 가져와서 카드 이름 출력 (현재 주석처리됨)
            TMP_Text tmpText = buttonObj.GetComponentInChildren<TMP_Text>();
            //tmpText.text = currentCards[i].cardName;
        }

        // 디버그 로그로 카드 이름 출력 (정상 뽑혔는지 확인용)
        for (int i = 0; i < currentCards.Length; i++)
            Debug.Log($"[{i}] {currentCards[i].cardName}");
    }

    /// <summary>
    /// 이벤트 UI 닫기
    /// </summary>
    private void CloseEventUI()
    {
        eventCanvas.SetActive(false);
    }

    /// <summary>
    /// 카드 선택 시 호출
    /// 선택한 카드의 기능을 실행하고 UI를 닫기
    /// </summary>
    /// <param name="index">선택한 카드의 인덱스</param>
    public void SelectCard(int index)
    {
        // 유효한 범위 체크 (예외 방지)
        if (index < 0 || index >= currentCards.Length) return;

        EventCardData selectedCard = currentCards[index];
        Debug.Log($"카드 '{selectedCard.cardName}' 선택됨");

        // 카드의 기능 실행
        EventFunctionManager.Instance.Execute(selectedCard.functionKey);

        // 이벤트 UI 닫기
        CloseEventUI();
    }
}

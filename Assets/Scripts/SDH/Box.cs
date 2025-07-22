using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Box : MonoBehaviour
{
    private Card2D card;
    private List<Card2D> childCards = new List<Card2D>();
    [SerializeField] private GameObject boxUI;  // 연결할 UI 패널
    [SerializeField] private Transform contentParent;    // ScrollView Content (카드 UI가 붙는 부모)
    [SerializeField] private GameObject cardUIPrefab;    // 카드 UI Prefab (CardUI)
    private int lastCardCount;

    private void Start()
    {
        card = GetComponent<Card2D>();

        if (boxUI != null)
            boxUI.SetActive(false);  // 시작할 때 UI 꺼두기

        // 최초 카드 개수 저장
        lastCardCount = GetComponentsInChildren<Card2D>(true)
            .Where(c => c != card).Count();

        // 초기 데이터 및 UI 갱신
        UpdateBoxData();
        UpdateUI();
    }

    private void Update()
    {
        int currentCardCount = GetComponentsInChildren<Card2D>(true)
        .Where(c => c != card).Count();

        // 카드 개수 변화 감지
        if (lastCardCount != currentCardCount)
        {
            lastCardCount = currentCardCount;

            UpdateBoxData();
            UpdateUI();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (boxUI != null)
                boxUI.SetActive(!boxUI.activeSelf);  // 토글
        }
    }

    private void UpdateBoxData()
    {
        // 자기 자신(Card2D)은 제외하고 자식, 손자까지 모두 가져옴
        childCards = GetComponentsInChildren<Card2D>(true)
            .Where(c => c != card)
            .ToList();

        foreach (var childCard in childCards)
        {
            childCard.gameObject.SetActive(false);
        }
    }

    private void UpdateUI()
    {
        // 기존 UI 전부 삭제
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 카드 리스트만큼 UI 생성 및 이름 세팅
        foreach (var childCard in childCards)
        {
            GameObject go = Instantiate(cardUIPrefab, contentParent);

            // CardUI 컴포넌트 가져오기
            var cardUI = go.GetComponent<CardUI>();
            if (cardUI != null)
            {
                cardUI.linkedCard = childCard;  // 실제 카드 연결
                cardUI.box = this;               // 박스 연결
            }

            // TMP_Text 가져와서 이름 세팅
            TMP_Text tmp = go.GetComponentInChildren<TMP_Text>();
            if (tmp != null)
            {
                tmp.text = childCard.cardData.cardName;
            }
        }
    }

    public void RemoveCard(Card2D card)
    {
        childCards.Remove(card);

        // Box의 부모의 자식(FieldCardsS)으로 옮기기
        card.transform.SetParent(this.transform.parent);

        card.gameObject.SetActive(true); // 필드에 다시 활성화

        UpdateUI();
    }
}

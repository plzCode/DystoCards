using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Card_Box : MonoBehaviour
{
    private Transform contentParent; // ScrollView Content (카드 UI가 붙는 부모)
    private GameObject cardUIPrefab; // 카드 UI Prefab (CardUI)
    private BoxManager boxManager;
    private List<Card2D> childCards = new List<Card2D>();
    private Card2D card;
    private int lastCardCount;
    private float lastClickTime;
    private const float doubleClickThreshold = 0.3f;

    private void Start()
    {
        card = GetComponent<Card2D>();

        // 싱글톤에서 참조로 가져옴
        contentParent = BoxManager.Instance.ContentParent;
        cardUIPrefab = BoxManager.Instance.CardUIPrefab;
        boxManager = BoxManager.Instance;

        if (boxManager != null)
            boxManager.CloseUI(); // 시작할 때 UI 꺼두기

        // 최초 카드 개수 저장
        lastCardCount = GetComponentsInChildren<Card2D>(true)
            .Where(c => c != card).Count();

        // 초기 데이터 및 UI 갱신
        UpdateBoxData();
        UpdateCardUI();
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
            UpdateCardUI();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                float timeSinceLastClick = Time.time - lastClickTime;
                lastClickTime = Time.time;

                if (timeSinceLastClick < doubleClickThreshold)
                {
                    if (boxManager != null)
                        boxManager.OpenUI(); // UI 활성화
                }
            }
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
            if (childCard.transform.parent != this.transform)
                childCard.transform.SetParent(this.transform);

            childCard.gameObject.SetActive(false);
        }
    }

    private void UpdateCardUI()
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
            var cardUI = go.GetComponent<BoxCardUI>();
            if (cardUI != null)
            {
                cardUI.linkedCard = childCard; // 실제 카드 연결
                cardUI.box = this; // 박스 연결
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

        // 카드 부모를 필드로 설정 후 활성화
        card.transform.SetParent(this.transform.parent);
        card.gameObject.SetActive(true);
    }
}

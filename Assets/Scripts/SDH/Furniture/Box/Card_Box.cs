using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    private int maxSize;

    private readonly Dictionary<string, int> boxSizeMap = new Dictionary<string, int>()
    {
        { "WoodBox", 10 },
        { "SteelBox", 20 }, 
        { "Refrigerator", 20 },
    };

    private void Start()
    {
        card = GetComponent<Card2D>();
        if (boxSizeMap.TryGetValue(card.RuntimeData.cardName, out int size))
        {
            maxSize = size;
            if (card.RuntimeData.cardName == "Refrigerator")
                TurnManager.Instance.RegisterPhaseAction(TurnPhase.DayAction, () => UpdateRefrigeratorItems());
        }
        else
            maxSize = 5; // 기본값 또는 예외 처리

        // 싱글톤에서 참조로 가져옴
        contentParent = BoxManager.Instance.ContentParent;
        cardUIPrefab = BoxManager.Instance.CardUIPrefab;
        boxManager = BoxManager.Instance;

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
                        boxManager.OpenUI(this); // UI 활성화
                }
            }
        }
    }

    public void UpdateBoxData()
    {
        // 자기 자신(Card2D)은 제외하고 자식, 손자까지 모두 가져옴
        childCards = GetComponentsInChildren<Card2D>(true)
            .Where(c => c != card)
            .ToList();

        int totalSize = 0;

        if (card.RuntimeData.cardName == "Refrigerator")
        {
            // 음식이 아닌 카드 먼저 제거
            foreach (var c in childCards.ToList())
            {
                if (c.RuntimeData.cardType != CardType.Food)
                    RemoveCard(c);
            }
        }

        foreach (var childCard in childCards)
        {
            int cardSize = childCard.RuntimeData != null ? childCard.RuntimeData.size : 0;

            // 누적 size가 maxSize 이상이면 더 이상 처리하지 않음
            if (totalSize + cardSize > maxSize)
            {
                RemoveCard(childCard);
                break;
            }
            totalSize += cardSize;
                
            if (childCard.transform.parent != this.transform)
                childCard.transform.SetParent(this.transform);

            // 시각적으로 숨김
            var renderer = childCard.GetComponent<SpriteRenderer>();
            if (renderer != null)
                renderer.enabled = false;

            // Collider2D 끄기
            var collider = childCard.GetComponent<Collider2D>();
            if (collider != null)
                collider.enabled = false;
        }

        // 차지량 UI 텍스트 갱신
        if (boxManager != null && boxManager.CapacityText != null)
        {
            boxManager.CapacityText.text = $"{totalSize} / {maxSize}";
        }
    }

    public  void UpdateCardUI()
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
                tmp.text = childCard.RuntimeData.cardName;
            }
        }
    }

    public void RemoveCard(Card2D card)
    {
        childCards.Remove(card);

        // 카드 부모를 필드로 설정 후 활성화
        card.transform.SetParent(this.transform.parent);

        // 다시 보이도록 설정
        var renderer = card.GetComponent<SpriteRenderer>();
        if (renderer != null)
            renderer.enabled = true;

        var collider = card.GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = true;
    }

    private void UpdateRefrigeratorItems()
    {
        if (card.RuntimeData.cardName != "Refrigerator")
            return;

        foreach (var childCard in childCards)
        {
            // cardData를 FoodCardData로 안전하게 형변환
            if (childCard.RuntimeData is FoodCardData foodCardData)
            {
                foodCardData.shelfLifeTurns++;
            }
        }
    }
}

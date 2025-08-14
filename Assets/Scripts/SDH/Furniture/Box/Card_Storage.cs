using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 카드 저장소 박스 클래스
/// 카드들을 담아 관리하며, UI 갱신과 카드 추가/제거 기능을 담당
/// 박스의 최대 용량에 따라 카드 수용을 제한하며, 냉장고 박스는 음식 카드에 특화된 기능을 가짐
/// 더블 클릭 시 저장소 UI 열림
public class Card_Storage : MonoBehaviour
{
    private Transform contentParent;                      // ScrollView Content (카드 UI가 붙는 부모)
    private GameObject cardUIPrefab;                      // 카드 UI 프리팹
    private StorageManager boxManager;                    // 저장소 매니저 (싱글톤)
    private List<Card2D> childCards = new List<Card2D>(); // 현재 박스에 들어있는 카드 리스트
    private Card2D card;                                  // 자신을 의미하는 카드
    private int lastCardCount;                            // 마지막 카드 개수 (변화 감지용)
    private float lastClickTime;                          // 더블 클릭 감지용 시간 기록
    private const float doubleClickThreshold = 0.3f;      // 더블 클릭 간격 임계값
    private int maxSize;                                  // 박스 최대 용량

    private bool isDragging = false;
    private bool hasInsertedBefore = false; // 박스에 물건이 한번이라도 들어갔는지 여부

    // 박스 이름에 따른 용량 맵
    private readonly Dictionary<string, int> boxSizeMap = new Dictionary<string, int>()
    {
        { "055", 10 }, // 나무 상자
        { "056", 20 }, // 철 상자
        { "052", 20 }, // 냉장고
    };

    private void Start()
    {
        // 자신의 Card2D 컴포넌트 가져오기
        card = GetComponent<Card2D>();

        // 이름에 따라 maxSize 설정, 냉장고면 DayEnd 이벤트 등록
        if (boxSizeMap.TryGetValue(card.RuntimeData.cardId, out int size))
        {
            maxSize = size;

            if (card.RuntimeData.cardId == "052")
                TurnManager.Instance.RegisterPhaseAction(TurnPhase.DayEnd, () => UpdateRefrigeratorItems());
        }
        else
        {
            maxSize = 5; // 기본 용량
        }

        // 싱글톤 참조 설정
        contentParent = StorageManager.Instance.ContentParent;
        cardUIPrefab = StorageManager.Instance.CardUIPrefab;
        boxManager = StorageManager.Instance;

        // 시작 시 카드 개수 저장
        lastCardCount = GetComponentsInChildren<Card2D>(true)
            .Where(c => c != card).Count();

        // 초기 박스 데이터 및 UI 갱신
        UpdateBoxData();
        UpdateCardUI();
    }

    private void Update()
    {
        // 드래그 여부는 항상 체크
        if (card.isDragging)
            isDragging = true;

        // 현재 카드 개수 확인
        int currentCardCount = GetComponentsInChildren<Card2D>(true)
            .Where(c => c != card).Count();

        // 카드 개수가 바뀌었을 경우
        if (lastCardCount != currentCardCount)
        {
            bool isFirstInsert = !hasInsertedBefore && currentCardCount > 0;

            // 첫 삽입이거나, 드래그 중이 아닐 때만 소리 재생
            if (isFirstInsert || (!isDragging && currentCardCount > lastCardCount))
            {
                AudioManager.Instance.PlaySFX("StorageIn");
                hasInsertedBefore = true; // 첫 삽입 기록
            }

            // 드래그 중이었다면 드래그 상태 해제
            if (isDragging && currentCardCount > lastCardCount)
                isDragging = false;

            lastCardCount = currentCardCount;

            // 박스 상태 및 UI 갱신
            UpdateBoxData();
            UpdateCardUI();
        }

        // 마우스 왼쪽 클릭 감지
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 위치 -> 월드 좌표로 변환
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            // 자신이 클릭되었는지 확인
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                float timeSinceLastClick = Time.time - lastClickTime;
                lastClickTime = Time.time;

                // 더블 클릭이면 UI 열기
                if (timeSinceLastClick < doubleClickThreshold)
                {
                    if (boxManager != null)
                        boxManager.OpenUI(this); // 박스 UI 열기
                }
            }
        }
    }

    /// <summary>
    /// 박스 내부 카드 정보 갱신
    /// </summary>
    public void UpdateBoxData()
    {
        // 자기 자신 제외한 자식 카드 리스트 업데이트
        childCards = GetComponentsInChildren<Card2D>(true)
            .Where(c => c != card)
            .ToList();

        int totalSize = 0;

        // 냉장고라면 음식 카드만 남김
        if (card.RuntimeData.cardId == "052")
        {
            foreach (var c in childCards.ToList())
            {
                if (c.RuntimeData.cardType != CardType.Food)
                    RemoveCard(c);
            }
        }
        else if (card.RuntimeData.cardId == "055" || card.RuntimeData.cardId == "056")
        {
            foreach (var c in childCards.ToList())
            {
                if (c.RuntimeData.cardType != CardType.Resource && 
                    c.RuntimeData.cardType != CardType.Food && 
                    c.RuntimeData.cardType != CardType.Equipment && 
                    c.RuntimeData.cardType != CardType.Heal)
                    RemoveCard(c);
            }
        }

        // 카드 하나씩 처리
        foreach (var childCard in childCards)
        {
            int cardSize = childCard.RuntimeData != null ? childCard.RuntimeData.size : 0;

            // 최대 용량 초과 시 카드 제거
            if (totalSize + cardSize > maxSize)
            {
                RemoveCard(childCard);
                break;
            }

            totalSize += cardSize;

            //제외할 컴포넌트 미리 등록
            Canvas childCanvas = childCard.GetComponentInChildren<Canvas>();
            Image image = childCanvas.gameObject.GetComponentInChildren<Image>();

            // 부모가 다르면 현재 박스로 설정
            if (childCard.transform.parent != this.transform)
                childCard.transform.SetParent(this.transform);

            // 카드 비활성화 (시각적으로만)
            childCard.gameObject.SetActive(true);
            var renderer = childCard.GetComponent<SpriteRenderer>();
            if (renderer != null)
                renderer.enabled = false;

            var collider = childCard.GetComponent<Collider2D>();
            if (collider != null)
                collider.enabled = false;

            // 하위 자식들도 비활성화
            foreach (Transform child in childCard.transform)
                child.gameObject.SetActive(false);

            if (childCanvas != null)
            {
                childCanvas.gameObject.SetActive(true);
                image.gameObject.SetActive(true);
                image.enabled = false;
            }
        }

        // UI 용량 텍스트 갱신
        if (boxManager != null && boxManager.CapacityText != null)
        {
            boxManager.CapacityText.text = $"{totalSize} / {maxSize}";
        }
    }

    /// <summary>
    /// 카드 UI 갱신 (ScrollView에 표시)
    /// </summary>
    public void UpdateCardUI()
    {
        // 기존 UI 전부 제거
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 각 카드에 대해 UI 항목 생성
        foreach (var childCard in childCards)
        {
            GameObject go = Instantiate(cardUIPrefab, contentParent);

            // StorageCardUI 설정
            var cardUI = go.GetComponent<StorageCardUI>();
            if (cardUI != null)
            {
                cardUI.linkedCard = childCard;
                cardUI.box = this;
            }

            Image img = go.GetComponentInChildren<Image>();
            if (img != null)
            {
                var cardRenderer = childCard.GetComponent<CardUIRenderer>();
                if (cardRenderer != null)
                {
                    img.sprite = cardRenderer.cardImage.sprite; // childCard의 CardUIRenderer에서 sprite 가져오기
                }
            }
        }
    }

    /// <summary>
    /// 카드 제거 시 호출 (박스에서 꺼냄)
    /// </summary>
    public void RemoveCard(Card2D card)
    {
        childCards.Remove(card);

        // 카드 비활성화 (시각적으로만)
        card.gameObject.SetActive(true);

        // 부모를 원래 위치로
        card.transform.SetParent(this.transform.parent);

        // 시각적으로 다시 표시
        var renderer = card.GetComponent<SpriteRenderer>();
        if (renderer != null)
            renderer.enabled = true;

        var collider = card.GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = true;

        // 자식들 활성화
        foreach (Transform child in card.transform)
            child.gameObject.SetActive(true);

        card.gameObject.GetComponentInChildren<Canvas>().GetComponentInChildren<Image>().enabled = true;

        AudioManager.Instance.PlaySFX("StorageOut");
    }

    /// <summary>
    /// 냉장고에 들어간 음식카드 유통기한 증가
    /// </summary>
    private void UpdateRefrigeratorItems()
    {
        if (card.RuntimeData.cardId != "052")
            return;

        foreach (var childCard in childCards)
        {
            // FoodCardData로 형변환 후 shelfLifeTurns 증가
            if (childCard.RuntimeData is FoodCardData foodCardData)
            {
                foodCardData.shelfLifeTurns++;
            }
        }

        TurnManager.Instance.MarkActionComplete();
    }
}

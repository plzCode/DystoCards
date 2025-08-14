using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ī�� ����� �ڽ� Ŭ����
/// ī����� ��� �����ϸ�, UI ���Ű� ī�� �߰�/���� ����� ���
/// �ڽ��� �ִ� �뷮�� ���� ī�� ������ �����ϸ�, ����� �ڽ��� ���� ī�忡 Ưȭ�� ����� ����
/// ���� Ŭ�� �� ����� UI ����
public class Card_Storage : MonoBehaviour
{
    private Transform contentParent;                      // ScrollView Content (ī�� UI�� �ٴ� �θ�)
    private GameObject cardUIPrefab;                      // ī�� UI ������
    private StorageManager boxManager;                    // ����� �Ŵ��� (�̱���)
    private List<Card2D> childCards = new List<Card2D>(); // ���� �ڽ��� ����ִ� ī�� ����Ʈ
    private Card2D card;                                  // �ڽ��� �ǹ��ϴ� ī��
    private int lastCardCount;                            // ������ ī�� ���� (��ȭ ������)
    private float lastClickTime;                          // ���� Ŭ�� ������ �ð� ���
    private const float doubleClickThreshold = 0.3f;      // ���� Ŭ�� ���� �Ӱ谪
    private int maxSize;                                  // �ڽ� �ִ� �뷮

    private bool isDragging = false;
    private bool hasInsertedBefore = false; // �ڽ��� ������ �ѹ��̶� ������ ����

    // �ڽ� �̸��� ���� �뷮 ��
    private readonly Dictionary<string, int> boxSizeMap = new Dictionary<string, int>()
    {
        { "055", 10 }, // ���� ����
        { "056", 20 }, // ö ����
        { "052", 20 }, // �����
    };

    private void Start()
    {
        // �ڽ��� Card2D ������Ʈ ��������
        card = GetComponent<Card2D>();

        // �̸��� ���� maxSize ����, ������ DayEnd �̺�Ʈ ���
        if (boxSizeMap.TryGetValue(card.RuntimeData.cardId, out int size))
        {
            maxSize = size;

            if (card.RuntimeData.cardId == "052")
                TurnManager.Instance.RegisterPhaseAction(TurnPhase.DayEnd, () => UpdateRefrigeratorItems());
        }
        else
        {
            maxSize = 5; // �⺻ �뷮
        }

        // �̱��� ���� ����
        contentParent = StorageManager.Instance.ContentParent;
        cardUIPrefab = StorageManager.Instance.CardUIPrefab;
        boxManager = StorageManager.Instance;

        // ���� �� ī�� ���� ����
        lastCardCount = GetComponentsInChildren<Card2D>(true)
            .Where(c => c != card).Count();

        // �ʱ� �ڽ� ������ �� UI ����
        UpdateBoxData();
        UpdateCardUI();
    }

    private void Update()
    {
        // �巡�� ���δ� �׻� üũ
        if (card.isDragging)
            isDragging = true;

        // ���� ī�� ���� Ȯ��
        int currentCardCount = GetComponentsInChildren<Card2D>(true)
            .Where(c => c != card).Count();

        // ī�� ������ �ٲ���� ���
        if (lastCardCount != currentCardCount)
        {
            bool isFirstInsert = !hasInsertedBefore && currentCardCount > 0;

            // ù �����̰ų�, �巡�� ���� �ƴ� ���� �Ҹ� ���
            if (isFirstInsert || (!isDragging && currentCardCount > lastCardCount))
            {
                AudioManager.Instance.PlaySFX("StorageIn");
                hasInsertedBefore = true; // ù ���� ���
            }

            // �巡�� ���̾��ٸ� �巡�� ���� ����
            if (isDragging && currentCardCount > lastCardCount)
                isDragging = false;

            lastCardCount = currentCardCount;

            // �ڽ� ���� �� UI ����
            UpdateBoxData();
            UpdateCardUI();
        }

        // ���콺 ���� Ŭ�� ����
        if (Input.GetMouseButtonDown(0))
        {
            // ���콺 ��ġ -> ���� ��ǥ�� ��ȯ
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            // �ڽ��� Ŭ���Ǿ����� Ȯ��
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                float timeSinceLastClick = Time.time - lastClickTime;
                lastClickTime = Time.time;

                // ���� Ŭ���̸� UI ����
                if (timeSinceLastClick < doubleClickThreshold)
                {
                    if (boxManager != null)
                        boxManager.OpenUI(this); // �ڽ� UI ����
                }
            }
        }
    }

    /// <summary>
    /// �ڽ� ���� ī�� ���� ����
    /// </summary>
    public void UpdateBoxData()
    {
        // �ڱ� �ڽ� ������ �ڽ� ī�� ����Ʈ ������Ʈ
        childCards = GetComponentsInChildren<Card2D>(true)
            .Where(c => c != card)
            .ToList();

        int totalSize = 0;

        // ������� ���� ī�常 ����
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

        // ī�� �ϳ��� ó��
        foreach (var childCard in childCards)
        {
            int cardSize = childCard.RuntimeData != null ? childCard.RuntimeData.size : 0;

            // �ִ� �뷮 �ʰ� �� ī�� ����
            if (totalSize + cardSize > maxSize)
            {
                RemoveCard(childCard);
                break;
            }

            totalSize += cardSize;

            //������ ������Ʈ �̸� ���
            Canvas childCanvas = childCard.GetComponentInChildren<Canvas>();
            Image image = childCanvas.gameObject.GetComponentInChildren<Image>();

            // �θ� �ٸ��� ���� �ڽ��� ����
            if (childCard.transform.parent != this.transform)
                childCard.transform.SetParent(this.transform);

            // ī�� ��Ȱ��ȭ (�ð������θ�)
            childCard.gameObject.SetActive(true);
            var renderer = childCard.GetComponent<SpriteRenderer>();
            if (renderer != null)
                renderer.enabled = false;

            var collider = childCard.GetComponent<Collider2D>();
            if (collider != null)
                collider.enabled = false;

            // ���� �ڽĵ鵵 ��Ȱ��ȭ
            foreach (Transform child in childCard.transform)
                child.gameObject.SetActive(false);

            if (childCanvas != null)
            {
                childCanvas.gameObject.SetActive(true);
                image.gameObject.SetActive(true);
                image.enabled = false;
            }
        }

        // UI �뷮 �ؽ�Ʈ ����
        if (boxManager != null && boxManager.CapacityText != null)
        {
            boxManager.CapacityText.text = $"{totalSize} / {maxSize}";
        }
    }

    /// <summary>
    /// ī�� UI ���� (ScrollView�� ǥ��)
    /// </summary>
    public void UpdateCardUI()
    {
        // ���� UI ���� ����
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // �� ī�忡 ���� UI �׸� ����
        foreach (var childCard in childCards)
        {
            GameObject go = Instantiate(cardUIPrefab, contentParent);

            // StorageCardUI ����
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
                    img.sprite = cardRenderer.cardImage.sprite; // childCard�� CardUIRenderer���� sprite ��������
                }
            }
        }
    }

    /// <summary>
    /// ī�� ���� �� ȣ�� (�ڽ����� ����)
    /// </summary>
    public void RemoveCard(Card2D card)
    {
        childCards.Remove(card);

        // ī�� ��Ȱ��ȭ (�ð������θ�)
        card.gameObject.SetActive(true);

        // �θ� ���� ��ġ��
        card.transform.SetParent(this.transform.parent);

        // �ð������� �ٽ� ǥ��
        var renderer = card.GetComponent<SpriteRenderer>();
        if (renderer != null)
            renderer.enabled = true;

        var collider = card.GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = true;

        // �ڽĵ� Ȱ��ȭ
        foreach (Transform child in card.transform)
            child.gameObject.SetActive(true);

        card.gameObject.GetComponentInChildren<Canvas>().GetComponentInChildren<Image>().enabled = true;

        AudioManager.Instance.PlaySFX("StorageOut");
    }

    /// <summary>
    /// ����� �� ����ī�� ������� ����
    /// </summary>
    private void UpdateRefrigeratorItems()
    {
        if (card.RuntimeData.cardId != "052")
            return;

        foreach (var childCard in childCards)
        {
            // FoodCardData�� ����ȯ �� shelfLifeTurns ����
            if (childCard.RuntimeData is FoodCardData foodCardData)
            {
                foodCardData.shelfLifeTurns++;
            }
        }

        TurnManager.Instance.MarkActionComplete();
    }
}

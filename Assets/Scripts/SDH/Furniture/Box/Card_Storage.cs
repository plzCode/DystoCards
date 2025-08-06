using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Card_Storage : MonoBehaviour
{
    private Transform contentParent; // ScrollView Content (ī�� UI�� �ٴ� �θ�)
    private GameObject cardUIPrefab; // ī�� UI Prefab (CardUI)
    private StorageManager boxManager;
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
                TurnManager.Instance.RegisterPhaseAction(TurnPhase.DayEnd, () => UpdateRefrigeratorItems());
        }
        else
            maxSize = 5; // �⺻�� �Ǵ� ���� ó��

        // �̱��濡�� ������ ������
        contentParent = StorageManager.Instance.ContentParent;
        cardUIPrefab = StorageManager.Instance.CardUIPrefab;
        boxManager = StorageManager.Instance;

        // ���� ī�� ���� ����
        lastCardCount = GetComponentsInChildren<Card2D>(true)
            .Where(c => c != card).Count();
        
        // �ʱ� ������ �� UI ����
        UpdateBoxData();
        UpdateCardUI();
    }

    private void Update()
    {
        int currentCardCount = GetComponentsInChildren<Card2D>(true)
            .Where(c => c != card).Count();

        // ī�� ���� ��ȭ ����
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
                        boxManager.OpenUI(this); // UI Ȱ��ȭ
                }
            }
        }
    }

    public void UpdateBoxData()
    {
        // �ڱ� �ڽ�(Card2D)�� �����ϰ� �ڽ�, ���ڱ��� ��� ������
        childCards = GetComponentsInChildren<Card2D>(true)
            .Where(c => c != card)
            .ToList();

        int totalSize = 0;

        if (card.RuntimeData.cardName == "Refrigerator")
        {
            // ������ �ƴ� ī�� ���� ����
            foreach (var c in childCards.ToList())
            {
                if (c.RuntimeData.cardType != CardType.Food)
                    RemoveCard(c);
            }
        }

        foreach (var childCard in childCards)
        {
            int cardSize = childCard.RuntimeData != null ? childCard.RuntimeData.size : 0;

            // ���� size�� maxSize �̻��̸� �� �̻� ó������ ����
            if (totalSize + cardSize > maxSize)
            {
                RemoveCard(childCard);
                break;
            }
            totalSize += cardSize;
                
            if (childCard.transform.parent != this.transform)
                childCard.transform.SetParent(this.transform);

            // �ð������� ����
            var renderer = childCard.GetComponent<SpriteRenderer>();
            if (renderer != null)
                renderer.enabled = false;

            // Collider2D ����
            var collider = childCard.GetComponent<Collider2D>();
            if (collider != null)
                collider.enabled = false;

            for(int i = 0; i< childCard.transform.childCount; i++)
            {
                var child = childCard.transform.GetChild(i);
                child.gameObject.SetActive(false); // �ڽ� ī�嵵 ��Ȱ��ȭ
            }
        }

        // ������ UI �ؽ�Ʈ ����
        if (boxManager != null && boxManager.CapacityText != null)
        {
            boxManager.CapacityText.text = $"{totalSize} / {maxSize}";
        }
    }

    public  void UpdateCardUI()
    {
        // ���� UI ���� ����
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // ī�� ����Ʈ��ŭ UI ���� �� �̸� ����
        foreach (var childCard in childCards)
        {
            GameObject go = Instantiate(cardUIPrefab, contentParent);

            // CardUI ������Ʈ ��������
            var cardUI = go.GetComponent<StorageCardUI>();
            if (cardUI != null)
            {
                cardUI.linkedCard = childCard; // ���� ī�� ����
                cardUI.box = this; // �ڽ� ����
            }

            // TMP_Text �����ͼ� �̸� ����
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

        // ī�� �θ� �ʵ�� ���� �� Ȱ��ȭ
        card.transform.SetParent(this.transform.parent);

        // �ٽ� ���̵��� ����
        var renderer = card.GetComponent<SpriteRenderer>();
        if (renderer != null)
            renderer.enabled = true;

        var collider = card.GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = true;

        for (int i = 0; i < card.transform.childCount; i++)
        {
            var child = card.transform.GetChild(i);
            child.gameObject.SetActive(true); // �ڽ� ī�嵵 ��Ȱ��ȭ
        }
    }

    private void UpdateRefrigeratorItems()
    {
        if (card.RuntimeData.cardName != "Refrigerator")
            return;

        foreach (var childCard in childCards)
        {
            // cardData�� FoodCardData�� �����ϰ� ����ȯ
            if (childCard.RuntimeData is FoodCardData foodCardData)
            {
                foodCardData.shelfLifeTurns++;
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Card_Box : MonoBehaviour
{
    private Transform contentParent; // ScrollView Content (ī�� UI�� �ٴ� �θ�)
    private GameObject cardUIPrefab; // ī�� UI Prefab (CardUI)
    private BoxManager boxManager;
    private List<Card2D> childCards = new List<Card2D>();
    private Card2D card;
    private int lastCardCount;
    private float lastClickTime;
    private const float doubleClickThreshold = 0.3f;

    private void Start()
    {
        card = GetComponent<Card2D>();

        // �̱��濡�� ������ ������
        contentParent = BoxManager.Instance.ContentParent;
        cardUIPrefab = BoxManager.Instance.CardUIPrefab;
        boxManager = BoxManager.Instance;

        if (boxManager != null)
            boxManager.CloseUI(); // ������ �� UI ���α�

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
                        boxManager.OpenUI(); // UI Ȱ��ȭ
                }
            }
        }
    }

    private void UpdateBoxData()
    {
        // �ڱ� �ڽ�(Card2D)�� �����ϰ� �ڽ�, ���ڱ��� ��� ������
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
            var cardUI = go.GetComponent<BoxCardUI>();
            if (cardUI != null)
            {
                cardUI.linkedCard = childCard; // ���� ī�� ����
                cardUI.box = this; // �ڽ� ����
            }

            // TMP_Text �����ͼ� �̸� ����
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

        // ī�� �θ� �ʵ�� ���� �� Ȱ��ȭ
        card.transform.SetParent(this.transform.parent);
        card.gameObject.SetActive(true);
    }
}

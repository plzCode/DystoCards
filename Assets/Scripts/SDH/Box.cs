using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Box : MonoBehaviour
{
    private Card2D card;
    private List<Card2D> childCards = new List<Card2D>();
    [SerializeField] private GameObject boxUI;  // ������ UI �г�
    [SerializeField] private Transform contentParent;    // ScrollView Content (ī�� UI�� �ٴ� �θ�)
    [SerializeField] private GameObject cardUIPrefab;    // ī�� UI Prefab (CardUI)
    private int lastCardCount;

    private void Start()
    {
        card = GetComponent<Card2D>();

        if (boxUI != null)
            boxUI.SetActive(false);  // ������ �� UI ���α�

        // ���� ī�� ���� ����
        lastCardCount = GetComponentsInChildren<Card2D>(true)
            .Where(c => c != card).Count();

        // �ʱ� ������ �� UI ����
        UpdateBoxData();
        UpdateUI();
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
            UpdateUI();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (boxUI != null)
                boxUI.SetActive(!boxUI.activeSelf);  // ���
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
            childCard.gameObject.SetActive(false);
        }
    }

    private void UpdateUI()
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
            var cardUI = go.GetComponent<CardUI>();
            if (cardUI != null)
            {
                cardUI.linkedCard = childCard;  // ���� ī�� ����
                cardUI.box = this;               // �ڽ� ����
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

        // Box�� �θ��� �ڽ�(FieldCardsS)���� �ű��
        card.transform.SetParent(this.transform.parent);

        card.gameObject.SetActive(true); // �ʵ忡 �ٽ� Ȱ��ȭ

        UpdateUI();
    }
}

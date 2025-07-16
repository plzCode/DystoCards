using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �̺�Ʈ UI�� �����ϴ� Ŭ����
/// ī�� ���� �̺�Ʈ�� ó���ϸ�, EventFunctionManager�� ����
/// </summary>
public class EventUIManager : MonoBehaviour
{
    [SerializeField] private GameObject eventCanvas;      // �̺�Ʈ ī�� UI�� Canvas (��Ȱ��/Ȱ�� ��ȯ��)
    [SerializeField] private GameObject cardButtonPrefab; // ī�� ��ư ������ (��ư + TMP_Text ����)
    [SerializeField] private Transform buttonParent;      // ��ư���� ������ �θ� ������Ʈ (Grid Layout Group ��� ����)

    private List<GameObject> spawnedButtons = new List<GameObject>(); // ������ ��ư���� �����ϴ� ����Ʈ
    private EventCardData[] currentCards;                             // ���� ������ ī�� ������ �迭
    private int cardCount = 3;                                        // �� ���� ���� ī�� ���� (�⺻�� 3��)

    private void Start()
    {
        eventCanvas.SetActive(false); // ���� ���� �� �̺�Ʈ UI�� ��Ȱ��ȭ
    }

    private void Update()
    {
        // �׽�Ʈ��: T Ű�� ������ �̺�Ʈ UI�� ���� ���� ī�� ����
        if (Input.GetKeyDown(KeyCode.T))
        {
            currentCards = new EventCardData[0]; // �� �迭�� �ʱ�ȭ (Ȥ�� �� ���� ������ ����)
            OpenEventUI();
        }
    }

    /// <summary>
    /// �̺�Ʈ ī�� UI�� ���� ���� ī�带 �޾ƿ� ��ư�� ����
    /// </summary>
    private void OpenEventUI()
    {
        // ������ ������ ��ư���� ���� (�ߺ� ���� ����)
        foreach (var btn in spawnedButtons)
            Destroy(btn);
        
        spawnedButtons.Clear(); // ����Ʈ�� �ʱ�ȭ

        cardCount = 3; // �̹� �� ī�� ���� (���� ����, �ʿ� �� ����ȭ ����)
        currentCards = EventFunctionManager.Instance.GetRandomCards(cardCount); // ���� ī�� ��������

        eventCanvas.SetActive(true); // �̺�Ʈ ī�� UI Ȱ��ȭ

        // ī�� ��ư�� ���� �� �̺�Ʈ ���
        for (int i = 0; i < currentCards.Length; i++)
        {
            // ��ư �������� �����ؼ� �θ� ������Ʈ�� ���� (GridLayoutGroup�� ��ġ�� �ڵ� ����)
            GameObject buttonObj = Instantiate(cardButtonPrefab, buttonParent);
            spawnedButtons.Add(buttonObj); // ����Ʈ�� �߰�

            Button button = buttonObj.GetComponent<Button>(); // ��ư ������Ʈ ��������

            int index = i; // Ŭ���� ���� ������ ���� ���� (���� �ȿ��� ���)
            button.onClick.AddListener(() => SelectCard(index)); // ��ư Ŭ�� �� SelectCard ȣ��

            // ��ư �ȿ� �ִ� TMP_Text �����ͼ� ī�� �̸� ��� (���� �ּ�ó����)
            TMP_Text tmpText = buttonObj.GetComponentInChildren<TMP_Text>();
            //tmpText.text = currentCards[i].cardName;
        }

        // ����� �α׷� ī�� �̸� ��� (���� �������� Ȯ�ο�)
        for (int i = 0; i < currentCards.Length; i++)
            Debug.Log($"[{i}] {currentCards[i].cardName}");
    }

    /// <summary>
    /// �̺�Ʈ UI �ݱ�
    /// </summary>
    private void CloseEventUI()
    {
        eventCanvas.SetActive(false);
    }

    /// <summary>
    /// ī�� ���� �� ȣ��
    /// ������ ī���� ����� �����ϰ� UI�� �ݱ�
    /// </summary>
    /// <param name="index">������ ī���� �ε���</param>
    public void SelectCard(int index)
    {
        // ��ȿ�� ���� üũ (���� ����)
        if (index < 0 || index >= currentCards.Length) return;

        EventCardData selectedCard = currentCards[index];
        Debug.Log($"ī�� '{selectedCard.cardName}' ���õ�");

        // ī���� ��� ����
        EventFunctionManager.Instance.Execute(selectedCard.functionKey);

        // �̺�Ʈ UI �ݱ�
        CloseEventUI();
    }
}

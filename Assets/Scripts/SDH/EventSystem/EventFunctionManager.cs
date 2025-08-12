using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �̺�Ʈ ī���� ����� �����ϰ� ���� ī�带 �����ϴ� �Ŵ��� Ŭ����
/// Singleton �������� ���� ������ ����
/// </summary>
public class EventFunctionManager : MonoBehaviour
{
    public static EventFunctionManager Instance { get; private set; } // �̱��� �ν��Ͻ�
    [SerializeField] private List<EventCardData> eventCardDatabase;   // �̺�Ʈ ī�� �����ͺ��̽� (Inspector���� �Ҵ�)
    [SerializeField] private List<CardData> suppliesDatabase;
    [SerializeField] private List<HumanCardData> humanDatabase;

    private void Awake()
    {
        // Singleton ���� ����
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // �ߺ� �ν��Ͻ� ����
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // ���� �Ѿ�� ����
    }

    /// <summary>
    /// ������ �̺�Ʈ ī�带 �ϳ� ��ȯ�ϰ� �����ͺ��̽����� ����
    /// </summary>
    /// <returns>���� ī�� 1��</returns>
    public EventCardData GetRandomCard()
    {
        if (eventCardDatabase == null || eventCardDatabase.Count == 0)
        {
            Debug.LogWarning("�̺�Ʈ ī�� �����ͺ��̽��� ����ֽ��ϴ�.");
            return null;
        }

        int index = Random.Range(0, eventCardDatabase.Count);
        EventCardData selectedCard = eventCardDatabase[index];

        eventCardDatabase.RemoveAt(index); // ���� ī�� ����

        return selectedCard;
    }

    /// <summary>
    /// ī���� functionKey�� ���� �ش� �̺�Ʈ�� ����
    /// </summary>
    /// <param name="functionKey">�̺�Ʈ �Լ� Ű</param>
    public void Execute(string functionKey)
    {
        switch (functionKey)
        {
            case "Heal":
                Heal();
                break;
            case "Injure":
                Injure();
                break;
            case "SpawnEnemy":
                SpawnEnemy();
                break;
            case "ResourceGain":
                ResourceGain();
                break;
            case "RecruitHuman":
                RecruitHuman();
                break;
            default:
                Debug.LogWarning($"{functionKey}�� ��ϵ��� ���� �̺�Ʈ�Դϴ�.");
                break;
        }
    }

    // �÷��̾ ġ���ϴ� �̺�Ʈ ����
    private void Heal()
    {
        // �� �� �����ϴ� ��� Card2D ������Ʈ�� ã�Ƽ� �迭�� ������
        Card2D[] allCards = FindObjectsByType<Card2D>(FindObjectsSortMode.None);

        foreach (var card in allCards)
        {
            if (card.IsCharacterOfType(card.RuntimeData, CharacterType.Human))
            {
                Human human = card.GetComponent<Human>();
                if (human != null)
                    human.Heal(1);
            }
        }
    }

    private void Injure()
    {
        // �� �� �����ϴ� ��� Card2D ������Ʈ�� ã�Ƽ� �迭�� ������
        Card2D[] allCards = FindObjectsByType<Card2D>(FindObjectsSortMode.None);

        foreach (var card in allCards)
        {
            if (card.IsCharacterOfType(card.RuntimeData, CharacterType.Human))
            {
                Human human = card.GetComponent<Human>();
                if (human != null)
                    human.TakeDamage(1);
            }
        }
    }

    // ���� ��ȯ�ϴ� �̺�Ʈ ����
    private void SpawnEnemy()
    {
        BattleManager.Instance.SpawnMonster();
    }

    private void ResourceGain()
    {
        // �����ϰ� �ϳ� ����
        int randomIndex = Random.Range(0, suppliesDatabase.Count);
        CardData selectedData = suppliesDatabase[randomIndex];

        // ���ο� ī�� ����
        Card2D newCard = CardManager.Instance.SpawnCard(selectedData, Vector3.zero);
        newCard.BringToFrontRecursive(newCard); // ī�尡 ���� ���̵��� ����
        newCard.cardAnim.PlayFeedBack_ByName("BounceY"); // ���� �ִϸ��̼� ����
    }

    private void RecruitHuman()
    {
        // �����ϰ� �ϳ� ����
        int randomIndex = Random.Range(0, humanDatabase.Count);
        CardData selectedData = humanDatabase[randomIndex];

        // �����ͺ��̽����� �ش� human ����
        humanDatabase.RemoveAt(randomIndex);

        // ���ο� ī�� ����
        Card2D newCard = CardManager.Instance.SpawnCard(selectedData, Vector3.zero);
        newCard.BringToFrontRecursive(newCard); // ī�尡 ���� ���̵��� ����
        newCard.cardAnim.PlayFeedBack_ByName("BounceY"); // ���� �ִϸ��̼� ����
    }
}

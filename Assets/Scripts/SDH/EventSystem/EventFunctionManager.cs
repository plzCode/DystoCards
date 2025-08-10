using UnityEngine;

/// <summary>
/// �̺�Ʈ ī���� ����� �����ϰ� ���� ī�带 �����ϴ� �Ŵ��� Ŭ����
/// Singleton �������� ���� ������ ����
/// </summary>
public class EventFunctionManager : MonoBehaviour
{
    public static EventFunctionManager Instance { get; private set; } // �̱��� �ν��Ͻ�
    [SerializeField] private EventCardData[] eventCardDatabase;       // �̺�Ʈ ī�� �����ͺ��̽� (Inspector���� �Ҵ�)

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
    /// ������ �̺�Ʈ ī�带 �ϳ� ��ȯ
    /// </summary>
    /// <returns>���� ī�� 1��</returns>
    public EventCardData GetRandomCard()
    {
        if (eventCardDatabase == null || eventCardDatabase.Length == 0)
        {
            Debug.LogWarning("�̺�Ʈ ī�� �����ͺ��̽��� ����ֽ��ϴ�.");
            return null;
        }

        int index = Random.Range(0, eventCardDatabase.Length);
        return eventCardDatabase[index];
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
        Debug.Log("���� ��ȯ�մϴ�!");
        // ���� ��ȯ ������ ���⿡ �ۼ�
    }

    // ��带 �����ϴ� �̺�Ʈ ����
    private void ResourceGain()
    {
        Debug.Log("��带 �����մϴ�!");
        // ���� ��� ���� ������ ���⿡ �ۼ�
    }


    // ��带 �����ϴ� �̺�Ʈ ����
    private void RecruitHuman()
    {
        Debug.Log("��带 �����մϴ�!");
        // ���� ��� ���� ������ ���⿡ �ۼ�
    }
}

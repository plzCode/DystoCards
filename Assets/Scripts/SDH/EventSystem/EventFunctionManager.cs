using UnityEngine;

/// <summary>
/// 이벤트 카드의 기능을 실행하고 랜덤 카드를 제공하는 매니저 클래스
/// Singleton 패턴으로 전역 접근이 가능
/// </summary>
public class EventFunctionManager : MonoBehaviour
{
    public static EventFunctionManager Instance { get; private set; } // 싱글톤 인스턴스
    [SerializeField] private EventCardData[] eventCardDatabase;       // 이벤트 카드 데이터베이스 (Inspector에서 할당)

    private void Awake()
    {
        // Singleton 패턴 구현
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 중복 인스턴스 방지
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬이 넘어가도 유지
    }

    /// <summary>
    /// 랜덤한 이벤트 카드를 하나 반환
    /// </summary>
    /// <returns>랜덤 카드 1개</returns>
    public EventCardData GetRandomCard()
    {
        if (eventCardDatabase == null || eventCardDatabase.Length == 0)
        {
            Debug.LogWarning("이벤트 카드 데이터베이스가 비어있습니다.");
            return null;
        }

        int index = Random.Range(0, eventCardDatabase.Length);
        return eventCardDatabase[index];
    }

    /// <summary>
    /// 카드의 functionKey에 따라 해당 이벤트를 실행
    /// </summary>
    /// <param name="functionKey">이벤트 함수 키</param>
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
                Debug.LogWarning($"{functionKey}는 등록되지 않은 이벤트입니다.");
                break;
        }
    }

    // 플레이어를 치료하는 이벤트 실행
    private void Heal()
    {
        // 씬 내 존재하는 모든 Card2D 컴포넌트를 찾아서 배열로 가져옴
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
        // 씬 내 존재하는 모든 Card2D 컴포넌트를 찾아서 배열로 가져옴
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

    // 적을 소환하는 이벤트 실행
    private void SpawnEnemy()
    {
        Debug.Log("적을 소환합니다!");
        // 실제 소환 로직은 여기에 작성
    }

    // 골드를 지급하는 이벤트 실행
    private void ResourceGain()
    {
        Debug.Log("골드를 지급합니다!");
        // 실제 골드 지급 로직은 여기에 작성
    }


    // 골드를 지급하는 이벤트 실행
    private void RecruitHuman()
    {
        Debug.Log("골드를 지급합니다!");
        // 실제 골드 지급 로직은 여기에 작성
    }
}

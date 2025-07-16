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
    /// 중복 없이 랜덤한 이벤트 카드를 count개 반환
    /// </summary>
    /// <param name="count">뽑을 카드 개수</param>
    /// <returns>랜덤한 카드 배열</returns>
    public EventCardData[] GetRandomCards(int count)
    {
        // eventCardDatabase를 List로 복사하여 임시 리스트 생성
        var tempList = new System.Collections.Generic.List<EventCardData>(eventCardDatabase);
        var result = new System.Collections.Generic.List<EventCardData>();

        // count만큼 랜덤하게 카드를 뽑아 result에 추가
        for (int i = 0; i < count; i++)
        {
            if (tempList.Count == 0) break; // 남은 카드가 없으면 종료

            int index = Random.Range(0, tempList.Count); // 랜덤 인덱스
            result.Add(tempList[index]); // 카드 추가
            tempList.RemoveAt(index); // 중복 방지를 위해 삭제
        }

        return result.ToArray();
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
                HealPlayer();
                break;
            case "SpawnEnemy":
                SpawnEnemy();
                break;
            case "GiveGold":
                GiveGold();
                break;
            default:
                Debug.LogWarning($"{functionKey}는 등록되지 않은 이벤트입니다.");
                break;
        }
    }

    // 플레이어를 치료하는 이벤트 실행
    private void HealPlayer()
    {
        Debug.Log("플레이어를 치료합니다!");

        // 씬 내 존재하는 모든 Card2D 컴포넌트를 찾아서 배열로 가져옴
        Card2D[] allCards = FindObjectsByType<Card2D>(FindObjectsSortMode.None);

        foreach (var card in allCards)
        {
            if (card.IsCharacterOfType(card.cardData, CharacterType.Human))
            {
                Human human = card.GetComponent<Human>();
                if (human != null)
                    human.Heal(1);
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
    private void GiveGold()
    {
        Debug.Log("골드를 지급합니다!");
        // 실제 골드 지급 로직은 여기에 작성
    }
}

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 이벤트 카드의 기능을 실행하고 랜덤 카드를 제공하는 매니저 클래스
/// Singleton 패턴으로 전역 접근이 가능
/// </summary>
public class EventFunctionManager : MonoBehaviour
{
    public static EventFunctionManager Instance { get; private set; } // 싱글톤 인스턴스
    [SerializeField] private List<EventCardData> eventCardDatabase;   // 이벤트 카드 데이터베이스 (Inspector에서 할당)
    [SerializeField] private List<CardData> suppliesDatabase;
    [SerializeField] private List<HumanCardData> humanDatabase;

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
    /// 랜덤한 이벤트 카드를 하나 반환하고 데이터베이스에서 제거
    /// </summary>
    /// <returns>랜덤 카드 1개</returns>
    public EventCardData GetRandomCard()
    {
        if (eventCardDatabase == null || eventCardDatabase.Count == 0)
        {
            Debug.LogWarning("이벤트 카드 데이터베이스가 비어있습니다.");
            return null;
        }

        int index = Random.Range(0, eventCardDatabase.Count);
        EventCardData selectedCard = eventCardDatabase[index];

        eventCardDatabase.RemoveAt(index); // 뽑은 카드 삭제

        return selectedCard;
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
        BattleManager.Instance.SpawnMonster();
    }

    private void ResourceGain()
    {
        // 랜덤하게 하나 선택
        int randomIndex = Random.Range(0, suppliesDatabase.Count);
        CardData selectedData = suppliesDatabase[randomIndex];

        // 새로운 카드 생성
        Card2D newCard = CardManager.Instance.SpawnCard(selectedData, Vector3.zero);
        newCard.BringToFrontRecursive(newCard); // 카드가 위에 보이도록 정렬
        newCard.cardAnim.PlayFeedBack_ByName("BounceY"); // 생성 애니메이션 실행
    }

    private void RecruitHuman()
    {
        // 랜덤하게 하나 선택
        int randomIndex = Random.Range(0, humanDatabase.Count);
        CardData selectedData = humanDatabase[randomIndex];

        // 데이터베이스에서 해당 human 제거
        humanDatabase.RemoveAt(randomIndex);

        // 새로운 카드 생성
        Card2D newCard = CardManager.Instance.SpawnCard(selectedData, Vector3.zero);
        newCard.BringToFrontRecursive(newCard); // 카드가 위에 보이도록 정렬
        newCard.cardAnim.PlayFeedBack_ByName("BounceY"); // 생성 애니메이션 실행
    }
}

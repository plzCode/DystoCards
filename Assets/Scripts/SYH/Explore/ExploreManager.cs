using System.Collections.Generic;
using UnityEngine;

public class ExploreManager : MonoBehaviour
{

    public List<HumanCardData> registedHumans = new List<HumanCardData>();
    public static ExploreManager Instance { get; private set; }

    // 여러 탐험지를 저장
    public List<ExplorationData> registeredExplorations = new List<ExplorationData>();

    [SerializeField] private UIThemeData uiThemeData; // UI 색상 정보

    [SerializeField] private HumanScrollView humanScrollView;

    public event System.Action<ExplorationData> OnExploreAdded;
    public event System.Action<ExplorationData> OnExploreCompleted;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        UIBarUtility.Init(uiThemeData);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            humanScrollView.AddHuman(humanScrollView.exampleHuman1);
            registedHumans.Add(humanScrollView.exampleHuman1);
        }
    }

    private void Start()
    {
        TurnManager.Instance.RegisterPhaseAction(TurnPhase.ExploreAction, () => { });
        TurnManager.Instance.RegisterPhaseAction(TurnPhase.ExploreEnd,()=>ProcessExploreEnd());
    }

    public bool AddExplore(HumanCardData human, LocationInfo location)
    {
        foreach (var exploration in registeredExplorations)
        {
            if (exploration.human == human && exploration.location == location)
            {
                Debug.Log($"[중복 탐사] {human.cardName}은 이미 {location.locationName} 탐사 중입니다.");
                return false;
            }

            if (exploration.human == human)
            {
                Debug.Log($"[중복 탐사] {human.cardName}은 이미 다른 장소를 탐사 중입니다.");
                return false;
            }

            if (exploration.location == location)
            {
                Debug.Log($"[중복 탐사] {location.locationName}은 이미 다른 인물이 탐사 중입니다.");
                return false;
            }
        }

        ExplorationData newData = new ExplorationData(human, location);
        registeredExplorations.Add(newData);
        OnExploreAdded?.Invoke(newData); 
        Debug.Log($"[ExploreManager] 탐색 등록됨: {human.cardName} → {location.locationName}");
        return true;
    }

    private void ProcessExploreEnd()
    {
        List<ExplorationData> completed = new List<ExplorationData>();

        foreach (var data in registeredExplorations)
        {
            data.remainingDays--;
            Debug.Log($"[탐험 진행] {data.human.cardName} → {data.location.locationName}, 남은 일수: {data.remainingDays}");

            if (data.remainingDays <= 0)
            {
                Debug.Log($"[탐험 완료] {data.human.cardName}의 {data.location.locationName} 탐험이 완료되었습니다.");
                ProcessExploreResult(data.location, CaculateSuccessPercent(data.location, data.human));
                completed.Add(data);
            }
        }

        foreach (var data in completed)
        {
            registeredExplorations.Remove(data);
            OnExploreCompleted?.Invoke(data);  

        }
    }

    public float CaculateSuccessPercent(LocationInfo location, HumanCardData human)
    {
        if (location == null || human == null)
            return 0;

        if (human.stamina < location.requiredStamina)
            return 1;

        float baseSuccess = 100f;

        // 공격력 차이 보너스/패널티
        float strengthDiff = human.AttackPower - location.requiredStrength;
        float strengthModifier = 0f;

        if (strengthDiff >= 0)
        {
            strengthModifier = Mathf.Clamp(strengthDiff * 3f, 0f, 10f); // 보너스 최대 +10%
        }
        else
        {
            strengthModifier = Mathf.Clamp(strengthDiff * 20f, -100f, 0f); // 패널티 최대 -100%
        }

        // 위험도 패널티 (0~10 → 0~20%)
        float dangerPenalty = location.dangerLevel * 2f;

        // 허기 패널티
        float hungerPenalty = 0f;
        if (human.consume_hunger < location.durationDays)
        {
            float hungerLack = location.durationDays - human.consume_hunger;
            hungerPenalty = Mathf.Clamp(hungerLack * 5f, 0f, 30f); // 하루 부족당 -5%
        }

        // 최종 성공률
        float successRate = baseSuccess + strengthModifier - dangerPenalty - hungerPenalty;
        successRate = Mathf.Clamp(successRate, 5f, 95f); // 최소 5%, 최대 95%

        Debug.Log($"성공 확률 = {successRate}");

        return successRate;
    }

    public void ProcessExploreResult(LocationInfo location, float successRate)
    {
        List<CardData> givenRewards = new List<CardData>();

        foreach (var reward in location.rewards)
        {
            float roll = Random.Range(0f, 100f);
            if (roll <= reward.probability)
            {
                // 수량 보정
                float rawQuantity = reward.quantity * (successRate / 100f);
                int actualQuantity = Mathf.FloorToInt(rawQuantity);

                if (Random.value < rawQuantity - actualQuantity)
                    actualQuantity += 1;

                Debug.Log($"보상 지급: {reward.card.cardName} x {actualQuantity}");

                for (int i = 0; i < actualQuantity; i++)
                    givenRewards.Add(reward.card);
            }
        }

        // 추후 인벤토리 시스템 연결 가능
    }


    // 선택적으로: 전체 초기화 메서드
    public void ClearExplores()
    {
        registeredExplorations.Clear();
        Debug.Log("[ExploreManager] 모든 탐색 정보가 초기화됨.");
    }
}

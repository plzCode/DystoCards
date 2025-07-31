using System.Collections.Generic;
using UnityEngine;

public class ExploreManager : MonoBehaviour
{

    public List<HumanCardData> registedHumans = new List<HumanCardData>();
    public static ExploreManager Instance { get; private set; }

    // ���� Ž������ ����
    public List<ExplorationData> registeredExplorations = new List<ExplorationData>();

    [SerializeField] private UIThemeData uiThemeData; // UI ���� ����

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
                Debug.Log($"[�ߺ� Ž��] {human.cardName}�� �̹� {location.locationName} Ž�� ���Դϴ�.");
                return false;
            }

            if (exploration.human == human)
            {
                Debug.Log($"[�ߺ� Ž��] {human.cardName}�� �̹� �ٸ� ��Ҹ� Ž�� ���Դϴ�.");
                return false;
            }

            if (exploration.location == location)
            {
                Debug.Log($"[�ߺ� Ž��] {location.locationName}�� �̹� �ٸ� �ι��� Ž�� ���Դϴ�.");
                return false;
            }
        }

        ExplorationData newData = new ExplorationData(human, location);
        registeredExplorations.Add(newData);
        OnExploreAdded?.Invoke(newData); 
        Debug.Log($"[ExploreManager] Ž�� ��ϵ�: {human.cardName} �� {location.locationName}");
        return true;
    }

    private void ProcessExploreEnd()
    {
        List<ExplorationData> completed = new List<ExplorationData>();

        foreach (var data in registeredExplorations)
        {
            data.remainingDays--;
            Debug.Log($"[Ž�� ����] {data.human.cardName} �� {data.location.locationName}, ���� �ϼ�: {data.remainingDays}");

            if (data.remainingDays <= 0)
            {
                Debug.Log($"[Ž�� �Ϸ�] {data.human.cardName}�� {data.location.locationName} Ž���� �Ϸ�Ǿ����ϴ�.");
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

        // ���ݷ� ���� ���ʽ�/�г�Ƽ
        float strengthDiff = human.AttackPower - location.requiredStrength;
        float strengthModifier = 0f;

        if (strengthDiff >= 0)
        {
            strengthModifier = Mathf.Clamp(strengthDiff * 3f, 0f, 10f); // ���ʽ� �ִ� +10%
        }
        else
        {
            strengthModifier = Mathf.Clamp(strengthDiff * 20f, -100f, 0f); // �г�Ƽ �ִ� -100%
        }

        // ���赵 �г�Ƽ (0~10 �� 0~20%)
        float dangerPenalty = location.dangerLevel * 2f;

        // ��� �г�Ƽ
        float hungerPenalty = 0f;
        if (human.consume_hunger < location.durationDays)
        {
            float hungerLack = location.durationDays - human.consume_hunger;
            hungerPenalty = Mathf.Clamp(hungerLack * 5f, 0f, 30f); // �Ϸ� ������ -5%
        }

        // ���� ������
        float successRate = baseSuccess + strengthModifier - dangerPenalty - hungerPenalty;
        successRate = Mathf.Clamp(successRate, 5f, 95f); // �ּ� 5%, �ִ� 95%

        Debug.Log($"���� Ȯ�� = {successRate}");

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
                // ���� ����
                float rawQuantity = reward.quantity * (successRate / 100f);
                int actualQuantity = Mathf.FloorToInt(rawQuantity);

                if (Random.value < rawQuantity - actualQuantity)
                    actualQuantity += 1;

                Debug.Log($"���� ����: {reward.card.cardName} x {actualQuantity}");

                for (int i = 0; i < actualQuantity; i++)
                    givenRewards.Add(reward.card);
            }
        }

        // ���� �κ��丮 �ý��� ���� ����
    }


    // ����������: ��ü �ʱ�ȭ �޼���
    public void ClearExplores()
    {
        registeredExplorations.Clear();
        Debug.Log("[ExploreManager] ��� Ž�� ������ �ʱ�ȭ��.");
    }
}

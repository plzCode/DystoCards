using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExploreManager : MonoBehaviour
{

    public List<Human> registedHumans = new List<Human>();
    public static ExploreManager Instance { get; private set; }

    public List<MinimapIcon> mapLocationList;

    // ���� Ž������ ����
    public List<ExplorationData> registeredExplorations = new List<ExplorationData>();

    [SerializeField] private UIThemeData uiThemeData; // UI ���� ����

    [SerializeField] private ExploringScrollView addExploreScroll;
    [SerializeField] private HumanScrollView humanScrollView;
    [SerializeField] private RewardScrollView rewardScrollView;
    [SerializeField] private OpendLocationScroll opendLocationInfo;

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

        rewardScrollView.Init();

        mapLocationList.Sort((a, b) => a.locationInfo.openDay.CompareTo(b.locationInfo.openDay));
    }
    private void Update()
    {        

        if (Input.GetKeyDown(KeyCode.V))
        {
            CardManager.Instance.SpawnCardById("041", new Vector3(0, 0, 0));
        }
        
    }

    private void Start()
    {
        TurnManager.Instance.RegisterPhaseAction(TurnPhase.ExploreAction, () => registerHumanUpdate());
        TurnManager.Instance.RegisterPhaseAction(TurnPhase.ExploreEnd,()=>ProcessExploreEnd());
        TurnManager.Instance.RegisterPhaseAction(TurnPhase.DayAction, () => CheckOpenLocation());

        
    }
    void registerHumanUpdate()
    {
        registedHumans.Clear();
        List<Card2D> cards =  CardManager.Instance.GetCardsByType(CardType.Character);
        List<Card2D> humans = CardManager.Instance.GetCharacterType(cards, CharacterType.Human);
        for (int i = 0; i < humans.Count; i++)
        {
            registedHumans.Add(humans[i].gameObject.GetComponent<Human>());
        }

        addExploreScroll.gameObject.SetActive(true);
    }

    public bool AddExplore(Human human, LocationInfo location)
    {
        foreach (var exploration in registeredExplorations)
        {
            if (exploration.human == human && exploration.location == location)
            {
                Debug.Log($"[�ߺ� Ž��] {human.humanData.cardName}�� �̹� {location.locationName} Ž�� ���Դϴ�.");
                return false;
            }

            if (exploration.human == human)
            {
                Debug.Log($"[�ߺ� Ž��] {human.humanData.cardName}�� �̹� �ٸ� ��Ҹ� Ž�� ���Դϴ�.");
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
        Debug.Log($"[ExploreManager] Ž�� ��ϵ�: {human.humanData.cardName} �� {location.locationName}");
        return true;
    }

    private void ProcessExploreEnd()
    {
        List<ExplorationData> completed = new List<ExplorationData>();

        foreach (var data in registeredExplorations)
        {
            data.remainingDays--;
            Debug.Log($"[Ž�� ����] {data.human.humanData.cardName} �� {data.location.locationName}, ���� �ϼ�: {data.remainingDays}");

            if (data.remainingDays <= 0)
            {
                Debug.Log($"[Ž�� �Ϸ�] {data.human.humanData.cardName}�� {data.location.locationName} Ž���� �Ϸ�Ǿ����ϴ�.");
                //ProcessExploreResult(data.location, CaculateSuccessPercent(data.location, data.human));
                completed.Add(data);
            }
        }

        if (completed.Count==0)
        {
            rewardScrollView.gameObject.SetActive(true);
        }

        foreach (var data in completed)
        {
            data.human.ConsumeStamina(data.location.requiredStamina);
            registeredExplorations.Remove(data);            
            OnExploreCompleted?.Invoke(data);  

        }
    }

    public float CaculateSuccessPercent(LocationInfo location, HumanCardData human)
    {
        if (location == null || human == null)
            return 0;

        if (human.Stamina < location.requiredStamina)
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
        if (human.ConsumeHunger < location.durationDays)
        {
            float hungerLack = location.durationDays - human.ConsumeHunger;
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

    public List<RewardInfo> GetRewards(LocationInfo location, HumanCardData human)
    {
        float successRate = CaculateSuccessPercent(location, human);
        List<RewardInfo> givenRewards = new List<RewardInfo>();

        foreach (var reward in location.rewards)
        {
            float roll = Random.Range(0f, 100f);
            if (roll <= reward.probability)
            {
                RewardInfo calculatedRewards = new RewardInfo();
                float rawQuantity = reward.quantity * (successRate / 100f);
                int actualQuantity = Mathf.FloorToInt(rawQuantity);
                if (Random.value < rawQuantity - actualQuantity)
                    actualQuantity += 1;

                calculatedRewards.card = reward.card;
                calculatedRewards.quantity = actualQuantity;

                if (actualQuantity != 0)
                    givenRewards.Add(calculatedRewards);
            }
        }

        foreach (var rewardCard in givenRewards)
        {
            List<GameObject> spawnedObjects = new List<GameObject>();
            Vector3 groupTargetPosition = Vector3.zero;

            for (int i = 0; i < rewardCard.quantity; i++)
            {
                GameObject rewardObj = CardManager.Instance.SpawnCardById(rewardCard.card.cardId, Vector3.zero).gameObject;

                //  �ݶ��̴� ��Ȱ��ȭ
                var collider = rewardObj.GetComponent<BoxCollider2D>();
                if (collider != null) collider.enabled = false;

                Vector2 randomDir = Random.insideUnitCircle.normalized;
                float distance = Random.Range(1.5f, 3f);
                Vector3 targetOffset = (Vector3)(randomDir * distance);

                StartCoroutine(PopOutEffect(rewardObj.transform, targetOffset, 0.3f));

                if (i == 0)
                    groupTargetPosition = rewardObj.transform.position + targetOffset;

                spawnedObjects.Add(rewardObj);
            }

            // ���̰� ���� StackOnto + �ݶ��̴� �ٽ� Ȱ��ȭ
            StartCoroutine(MoveGroupToCenter(spawnedObjects, groupTargetPosition, delay: 1f, duration: 0.5f, onComplete: (groupList) =>
            {
                StartCoroutine(StackCardsSequentially(groupList, () =>
                {
                    // ��� ī���� �ݶ��̴� �ٽ� �ѱ�
                    foreach (var obj in groupList)
                    {
                        var collider = obj.GetComponent<BoxCollider2D>();
                        if (collider != null) collider.enabled = true;
                    }
                }));
            }));
        }

        return givenRewards;
    }

    IEnumerator PopOutEffect(Transform target, Vector2 moveOffset, float duration)
    {
        Vector3 startPos = target.position;
        Vector3 endPos = startPos + (Vector3)moveOffset;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float eased = 1f - Mathf.Pow(1f - t, 2f); // EaseOut
            target.position = Vector3.Lerp(startPos, endPos, eased);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.position = endPos;
    }

    IEnumerator MoveGroupToCenter(List<GameObject> groupObjects, Vector3 centerPosition, float delay, float duration, System.Action<List<GameObject>> onComplete = null)
    {
        yield return new WaitForSeconds(delay);

        int finishedCount = 0;

        foreach (var obj in groupObjects)
        {
            StartCoroutine(MoveToTarget(obj.transform, centerPosition, duration, () =>
            {
                finishedCount++;
            }));
        }

        // ���� �̵��� ������ ���
        while (finishedCount < groupObjects.Count)
        {
            yield return null;
        }

        // �� ���� �� �ݹ�
        onComplete?.Invoke(groupObjects);
    }

    IEnumerator MoveToTarget(Transform target, Vector3 endPos, float duration, System.Action onComplete)
    {
        Vector3 startPos = target.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float eased = t * t * (3f - 2f * t); // SmoothStep
            target.position = Vector3.Lerp(startPos, endPos, eased);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.position = endPos;
        onComplete?.Invoke();
    }

    IEnumerator StackCardsSequentially(List<GameObject> cards, System.Action onComplete = null)
    {
        if (cards == null || cards.Count < 2)
        {
            onComplete?.Invoke();
            yield break;
        }

        for (int i = 1; i < cards.Count; i++)
        {
            var from = cards[i].GetComponent<Card2D>();
            var to = cards[i - 1].GetComponent<Card2D>();

            if (from != null && to != null)
            {
                from.StackOnto(to);
            }

            yield return new WaitForSeconds(0.1f);
        }

        onComplete?.Invoke();
    }


    // ����������: ��ü �ʱ�ȭ �޼���
    public void ClearExplores()
    {
        registeredExplorations.Clear();
        Debug.Log("[ExploreManager] ��� Ž�� ������ �ʱ�ȭ��.");
    }

    void CheckOpenLocation()
    {
        int lastCount = -1;
        List<LocationInfo> opendLocations = new List<LocationInfo>();
        for (int i = 0; i < mapLocationList.Count; i++)
        {
            if (mapLocationList[i].locationInfo.openDay <= TurnManager.Instance.TurnCount)
            {
                lastCount = i;
                opendLocations.Add(mapLocationList[i].locationInfo);
                mapLocationList[i].SetInfo();
            }
            else
            {
                break;
            }
        }

        if (lastCount >= 0)
        {
            opendLocationInfo.ShowOpendLocationList(opendLocations);
        }


        if (lastCount >= 0 && lastCount < mapLocationList.Count)
        {
            mapLocationList.RemoveRange(0, lastCount + 1);
        }
    }
}

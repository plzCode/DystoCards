using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

[Serializable]
public enum TurnPhase
{
    EventDraw,
    DayAction,
    DayEnd,
    ExploreAction,
    ExploreEnd
}

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    public TurnPhase CurrentPhase { get; private set; } = TurnPhase.EventDraw;

    public event Action<TurnPhase> OnTurnPhaseChanged;
    private Dictionary<TurnPhase, Queue<Action>> phaseActions = new();

    public int TurnCount { get; private set; } = 1;
    public MMF_Player mmf_Player;

    [SerializeField] private bool isActionRunning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        RegisterTestActions();
        StartCoroutine(BeginTurnPhase(CurrentPhase));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            NextPhase();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            RegisterPhaseAction(TurnPhase.EventDraw, () => StartCoroutine(SampleAction("Custom action for Day Action phase.")));
        }
    }

    public void NextPhase()
    {
        CurrentPhase = GetNextPhase(CurrentPhase);
        Debug.Log($"[TurnManager] Phase changed to: {CurrentPhase}");
        OnTurnPhaseChanged?.Invoke(CurrentPhase);

        StartCoroutine(BeginTurnPhase(CurrentPhase));

        if (mmf_Player != null)
        {
            mmf_Player.PlayFeedbacks();
        }
    }

    private TurnPhase GetNextPhase(TurnPhase phase)
    {
        switch (phase)
        {
            case TurnPhase.EventDraw: return TurnPhase.DayAction;
            case TurnPhase.DayAction: return TurnPhase.DayEnd;
            case TurnPhase.DayEnd: return TurnPhase.ExploreAction;
            case TurnPhase.ExploreAction: return TurnPhase.ExploreEnd;
            case TurnPhase.ExploreEnd: TurnCount++; return TurnPhase.EventDraw;
            default: return TurnPhase.EventDraw;
        }
    }

    private IEnumerator BeginTurnPhase(TurnPhase phase)
    {
        // 자동 처리 로직
        switch (phase)
        {
            case TurnPhase.EventDraw:
                Debug.Log("Draw an event card.");
                break;
            case TurnPhase.DayAction:
                Debug.Log("Player can act during the day.");
                break;
            case TurnPhase.DayEnd:
                Debug.Log("Process end-of-day effects.");
                break;
            case TurnPhase.ExploreAction:
                Debug.Log("Player explores an area.");
                break;
            case TurnPhase.ExploreEnd:
                Debug.Log("Process exploration results.");
                break;
        }

        // 등록된 액션 순차 실행
        if (phaseActions.TryGetValue(phase, out var actionsQueue))
        {
            while (actionsQueue.Count > 0)
            {
                isActionRunning = true;
                var action = actionsQueue.Dequeue();
                action?.Invoke();
                yield return new WaitUntil(() => isActionRunning == false);
            }
        }
    }

    public void RegisterPhaseAction(TurnPhase phase, Action action)
    {
        if (!phaseActions.ContainsKey(phase))
            phaseActions[phase] = new Queue<Action>();

        phaseActions[phase].Enqueue(action);
    }

    public void UnregisterPhaseAction(TurnPhase phase, Action action)
    {
        // Queue에서는 특정 Action 제거가 복잡해서, 일반적으로 필요시 재구성
        Debug.LogWarning("[TurnManager] Unregister is not directly supported for queued actions.");
    }

    public void MarkActionComplete()
    {
        isActionRunning = false;
    }

    // 테스트 함수
    private IEnumerator SampleAction(string name)
    {
        Debug.Log($"{name} 시작");
        yield return new WaitForSeconds(2f);
        Debug.Log($"{name} 완료");
        //MarkActionComplete();
    }

    private void RegisterTestActions()
    {
        // 두 번째 액션: 플레이어가 스페이스 누르면 완료
        RegisterPhaseAction(TurnPhase.EventDraw, () =>
        {
            Debug.Log("Action 2 시작 - 스페이스 키를 누르면 완료");
            StartCoroutine(WaitForSpaceKey());
        });

        // 세 번째 액션: 즉시 완료
        RegisterPhaseAction(TurnPhase.EventDraw, () =>
        {
            Debug.Log("Action 3 시작 스페이스 키를 누르면 완료");
            StartCoroutine(WaitForSpaceKey());
        });
    }

    private IEnumerator WaitForSpaceKey()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        Debug.Log("스페이스 눌림 → 다음 액션으로");
        isActionRunning = false;
    }

}

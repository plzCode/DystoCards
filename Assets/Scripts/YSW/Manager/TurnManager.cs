using System;
using System.Collections.Generic;
using UnityEngine;

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
    private Dictionary<TurnPhase, Action> phaseStartActions = new();

    public int TurnCount { get; private set; } = 1;
    
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
        BeginTurnPhase(CurrentPhase);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            NextPhase();
        if( Input.GetKeyDown(KeyCode.Z))
        { 
            RegisterPhaseAction(TurnPhase.DayAction, () => Debug.Log("Custom action for Day Action phase."));
        }
    }

    public void NextPhase()
    {
        CurrentPhase = GetNextPhase(CurrentPhase);
        Debug.Log($"[TurnManager] Phase changed to: {CurrentPhase}");
        OnTurnPhaseChanged?.Invoke(CurrentPhase);
        BeginTurnPhase(CurrentPhase);
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

    private void BeginTurnPhase(TurnPhase phase)
    {
        // 각 Phase별로 자동 처리되는 로직이 있다면 여기에 정의
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
        //커스텀 등록된 액션 실행
        if (phaseStartActions.TryGetValue(phase, out var action))
        {
            action?.Invoke();
        }
    }

    public void RegisterPhaseAction(TurnPhase phase, Action action)
    {
        if (!phaseStartActions.ContainsKey(phase))
            phaseStartActions[phase] = action;
        else
            phaseStartActions[phase] += action;
    }
}

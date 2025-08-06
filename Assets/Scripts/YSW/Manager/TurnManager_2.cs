/*using System;
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

    private Dictionary<TurnPhase, List<Func<IEnumerator>>> phaseCoroutines = new();

    public int TurnCount { get; private set; } = 1;

    public MMF_Player mmf_Player;

    public bool userConfirmed = false;

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
        StartCoroutine(RunPhaseSequence(CurrentPhase));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            NextPhase();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            RegisterPhaseAction(TurnPhase.DayAction, WaitForUserConfirmExample);
        }
    }

    public void NextPhase()
    {
        CurrentPhase = GetNextPhase(CurrentPhase);
        Debug.Log($"[TurnManager] Phase changed to: {CurrentPhase}");
        OnTurnPhaseChanged?.Invoke(CurrentPhase);
        StartCoroutine(RunPhaseSequence(CurrentPhase));

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
            case TurnPhase.ExploreEnd:
                TurnCount++;
                return TurnPhase.EventDraw;
            default:
                return TurnPhase.EventDraw;
        }
    }

    private IEnumerator RunPhaseSequence(TurnPhase phase)
    {
        Debug.Log($"[TurnManager] Begin phase: {phase}");

        // 자동 처리 로직 (기본 메시지)
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

        // 등록된 코루틴 실행
        if (phaseCoroutines.TryGetValue(phase, out var actions))
        {
            foreach (var action in actions)
            {
                // 매 액션 실행 전에 userConfirmed 초기화
                userConfirmed = false;

                // 액션 자체를 실행
                yield return StartCoroutine(action());

                // 액션 종료를 기다림
                yield return new WaitUntil(() => userConfirmed);
            }
        }

        // 피드백
        if (mmf_Player != null)
        {
            mmf_Player.PlayFeedbacks();
        }
    }

    // 외부에서 호출되는 confirm 메서드
    public void ConfirmUserInput()
    {
        Debug.Log("User input confirmed.");
        userConfirmed = true;
    }

    // 예시로 등록하는 액션 함수
    public IEnumerator WaitForUserConfirmExample()
    {
        Debug.Log("[PhaseAction] 사용자 입력을 기다리는 중...");

        // 여기서 UI 버튼 등을 보여주고 외부에서 ConfirmUserInput()을 호출하게 만들 수 있음
        

        // 버튼 클릭 시 TurnManager.Instance.ConfirmUserInput() 호출됨

        yield break; // 종료는 여기서 하지 않고 WaitUntil이 다음 액션까지 대기함
    }

    public void RegisterPhaseAction(TurnPhase phase, Func<IEnumerator> coroutineAction)
    {
        if (!phaseCoroutines.ContainsKey(phase))
            phaseCoroutines[phase] = new List<Func<IEnumerator>>();
        phaseCoroutines[phase].Add(coroutineAction);
    }
}
*/
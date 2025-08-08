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
            // 테스트용 등록
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

        // 기본 자동 처리
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

        // 등록된 액션들 순차 실행
        if (phaseCoroutines.TryGetValue(phase, out var actions))
        {
            foreach (var action in actions)
            {
                userConfirmed = false; // 대기 시작

                yield return StartCoroutine(action());

                // ConfirmUserInput() 호출 대기
                yield return new WaitUntil(() => userConfirmed);
            }
        }

        // 피드백 재생
        if (mmf_Player != null)
        {
            mmf_Player.PlayFeedbacks();
        }

        Debug.Log($"[TurnManager] Phase {phase} actions completed.");
    }

    // 외부에서 호출해서 다음 액션 진행
    public void ConfirmUserInput()
    {
        Debug.Log("[TurnManager] User confirmed input.");
        userConfirmed = true;
    }

    // 예시 coroutine 액션
    public IEnumerator WaitForUserConfirmExample()
    {
        Debug.Log("유저 입력을 기다리는 액션 시작.");
        UIManager.Instance.ShowConfirmButton();  // 버튼 보여주기
        yield break;  // 액션 종료, WaitUntil로 다음 이동은 제어됨
    }

    public void RegisterPhaseAction(TurnPhase phase, Func<IEnumerator> coroutineAction)
    {
        if (!phaseCoroutines.ContainsKey(phase))
            phaseCoroutines[phase] = new List<Func<IEnumerator>>();
        phaseCoroutines[phase].Add(coroutineAction);
    }
    public void InsertPhaseAction(TurnPhase phase, int index, Func<IEnumerator> coroutineAction)
    {
        if (!phaseCoroutines.ContainsKey(phase))
        {
            phaseCoroutines[phase] = new List<Func<IEnumerator>>();
        }

        List<Func<IEnumerator>> actions = phaseCoroutines[phase];

        // 인덱스 유효성 검사
        if (index < 0 || index > actions.Count)
        {
            Debug.LogWarning($"[TurnManager] Invalid index {index} for inserting action into phase {phase}. Action will be appended.");
            actions.Add(coroutineAction);
        }
        else
        {
            actions.Insert(index, coroutineAction);
        }
    }
}
*/
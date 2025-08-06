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

        // �ڵ� ó�� ���� (�⺻ �޽���)
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

        // ��ϵ� �ڷ�ƾ ����
        if (phaseCoroutines.TryGetValue(phase, out var actions))
        {
            foreach (var action in actions)
            {
                // �� �׼� ���� ���� userConfirmed �ʱ�ȭ
                userConfirmed = false;

                // �׼� ��ü�� ����
                yield return StartCoroutine(action());

                // �׼� ���Ḧ ��ٸ�
                yield return new WaitUntil(() => userConfirmed);
            }
        }

        // �ǵ��
        if (mmf_Player != null)
        {
            mmf_Player.PlayFeedbacks();
        }
    }

    // �ܺο��� ȣ��Ǵ� confirm �޼���
    public void ConfirmUserInput()
    {
        Debug.Log("User input confirmed.");
        userConfirmed = true;
    }

    // ���÷� ����ϴ� �׼� �Լ�
    public IEnumerator WaitForUserConfirmExample()
    {
        Debug.Log("[PhaseAction] ����� �Է��� ��ٸ��� ��...");

        // ���⼭ UI ��ư ���� �����ְ� �ܺο��� ConfirmUserInput()�� ȣ���ϰ� ���� �� ����
        

        // ��ư Ŭ�� �� TurnManager.Instance.ConfirmUserInput() ȣ���

        yield break; // ����� ���⼭ ���� �ʰ� WaitUntil�� ���� �׼Ǳ��� �����
    }

    public void RegisterPhaseAction(TurnPhase phase, Func<IEnumerator> coroutineAction)
    {
        if (!phaseCoroutines.ContainsKey(phase))
            phaseCoroutines[phase] = new List<Func<IEnumerator>>();
        phaseCoroutines[phase].Add(coroutineAction);
    }
}
*/
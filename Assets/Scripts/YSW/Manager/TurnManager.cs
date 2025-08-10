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
        // �ڵ� ó�� ����
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

        // ��ϵ� �׼� ���� ����
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
        // Queue������ Ư�� Action ���Ű� �����ؼ�, �Ϲ������� �ʿ�� �籸��
        Debug.LogWarning("[TurnManager] Unregister is not directly supported for queued actions.");
    }

    public void MarkActionComplete()
    {
        isActionRunning = false;
    }

    // �׽�Ʈ �Լ�
    private IEnumerator SampleAction(string name)
    {
        Debug.Log($"{name} ����");
        yield return new WaitForSeconds(2f);
        Debug.Log($"{name} �Ϸ�");
        //MarkActionComplete();
    }

    private void RegisterTestActions()
    {
        // �� ��° �׼�: �÷��̾ �����̽� ������ �Ϸ�
        RegisterPhaseAction(TurnPhase.EventDraw, () =>
        {
            Debug.Log("Action 2 ���� - �����̽� Ű�� ������ �Ϸ�");
            StartCoroutine(WaitForSpaceKey());
        });

        // �� ��° �׼�: ��� �Ϸ�
        RegisterPhaseAction(TurnPhase.EventDraw, () =>
        {
            Debug.Log("Action 3 ���� �����̽� Ű�� ������ �Ϸ�");
            StartCoroutine(WaitForSpaceKey());
        });
    }

    private IEnumerator WaitForSpaceKey()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        Debug.Log("�����̽� ���� �� ���� �׼�����");
        isActionRunning = false;
    }

}

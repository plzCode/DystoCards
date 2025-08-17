using UnityEngine;
using System.Linq;
using System;

public class CharacterTaskRunner : MonoBehaviour
{
    public static CharacterTaskRunner Instance { get; private set; }

    private void Awake() => Instance = this;

    /* ------- TurnManager 이벤트 등록 ------- */
    private void Start()
    {
        TurnManager.Instance.RegisterPhaseAction(
            TurnPhase.DayAction, OnDayActionStart);

        TurnManager.Instance.RegisterPhaseAction(
            TurnPhase.DayEnd, OnDayEnd);
    }

    private void OnDayActionStart()
    {
        RecalcActionable(); // 기존 있던 계산 루틴 유지
        TurnManager.Instance.MarkActionComplete();
    }

    private void OnDayEnd()
    {
        foreach (var c in FindObjectsOfType<CharacterCard2D>())
            c.ClearCompleted();
        //DaySummary.Instance?.ShowPopupAndClear();
        HarvestWindow.Reset();
        TurnManager.Instance.MarkActionComplete();
    }

    public void RecalcActionable()
    {
        int n = FindObjectsOfType<CharacterCard2D>()
            .Count(c => c.HasAnyAvailable(ActionLibrary.GetActions(FacilityType.Farm)) ||
                        c.HasAnyAvailable(ActionLibrary.GetActions(FacilityType.Shelter)) ||
                        c.HasAnyAvailable(ActionLibrary.GetActions(FacilityType.ForestMine)));

        Debug.Log($"<color=yellow>[Runner] 작업 가능 인물 : {n}</color>");
    }

    public bool HasActionableCharacter()
    {
        return FindObjectsOfType<CharacterCard2D>()
            .Any(c => c.HasAnyAvailable(ActionLibrary.GetActions(FacilityType.Farm)) ||
                      c.HasAnyAvailable(ActionLibrary.GetActions(FacilityType.Shelter)) ||
                      c.HasAnyAvailable(ActionLibrary.GetActions(FacilityType.ForestMine)));
    }
}
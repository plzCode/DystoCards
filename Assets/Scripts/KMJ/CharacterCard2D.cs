using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CharacterCard2D : MonoBehaviour
{
    private Human human;
    private CharacterTaskRunner runner;
    private readonly HashSet<string> completed = new();
    private bool restedThisTurn = false;

    private void Awake() => human = GetComponent<Human>();

    public void SetRunner(CharacterTaskRunner r) => runner = r;

    public void ClearCompleted()
    {
        completed.Clear();
        restedThisTurn = false;                          
    }

    public bool CanDo(FacilityAction act)
    {
        if (restedThisTurn) return false;
        if (completed.Contains(act.id)) return false;

        bool ok;
        if (act.cost < 0) // 휴식(음수=회복)
            ok = !completed.Contains(act.id); // 최대치 미만일 때만
        else              // 작업(양수=소모)
            ok = !completed.Contains(act.id) && human.currentStamina >= act.cost;

        Debug.Log($"[CanDo] {name} vs {act.label}  " +
                  $"Stamina {human.currentStamina}/{act.cost}, done? {completed.Contains(act.id)} ⇒ {ok}");
        return ok;

    }

    public void DoAction(FacilityAction act)
    {
        if (completed.Contains(act.id) || restedThisTurn) return;

        if (human == null || human.humanData == null)
        {
            Debug.LogError("[Action] Human or humanData is null on " + name);
            return;
        }

        if (act.cost < 0)
        {              // 휴식: 비용이 음수면 회복량
            human.RecoverStamina(-act.cost);
            restedThisTurn = true;       // 그날 더 이상 작업 불가
        }
        else
        {
            if (human.currentStamina < act.cost) return; // 안전검사
            human.ConsumeStamina(act.cost);
        }

        completed.Add(act.id);
        runner?.RecalcActionable();
    }

    public bool HasAnyAvailable(IEnumerable<FacilityAction> acts)
    {
        foreach (var a in acts)
            if (CanDo(a)) return true;
        return false;
    }

    public void ResetTurn()
    {
        completed.Clear();
        restedThisTurn = false;
    }
}
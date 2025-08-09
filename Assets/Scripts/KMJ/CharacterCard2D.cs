using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CharacterCard2D : MonoBehaviour
{
    private Human human;
    private CharacterTaskRunner runner;
    private readonly HashSet<string> completed = new();

    private void Awake() => human = GetComponent<Human>();

    public void SetRunner(CharacterTaskRunner r) => runner = r;

    public void ClearCompleted() => completed.Clear();

    public bool CanDo(FacilityAction act)
    {
        bool ok = !completed.Contains(act.id) && human.currentStamina >= act.cost;

        Debug.Log($"[CanDo] {name} vs {act.label}  "
                  + $"Stamina {human.currentStamina}/{act.cost}, "
                  + $"done? {completed.Contains(act.id)}  ⇒ {ok}");

        return ok;

    }

    public void DoAction(FacilityAction act)
    {
        if (completed.Contains(act.id)) return;    // 같은 턴 중복 방지

        if (act.cost < 0)       // 휴식 : 회복
            human.currentStamina = Mathf.Min(
                human.currentStamina - act.cost,   // -(-2)=+2
                human.currentStamina + 999);       // 상한선 모르므로 큰 값
        else                    // 농사·제작 : 소모
            human.currentStamina = Mathf.Max(
                0, human.currentStamina - act.cost);

        Debug.Log($"[Stm] {name} → {human.currentStamina}");

        completed.Add(act.id);
        runner?.RecalcActionable();
    }

    public bool HasAnyAvailable(IEnumerable<FacilityAction> acts)
    {
        foreach (var a in acts)
            if (CanDo(a)) return true;
        return false;
    }

    public void ResetTurn() => completed.Clear();
}
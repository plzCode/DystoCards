using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CharacterCard2D : MonoBehaviour
{
    private Human human;
    private CharacterTaskRunner runner;
    private readonly HashSet<string> completed = new();
    private bool restedThisTurn = false;

    void Awake()
    {
        if (!human) human = GetComponent<Human>(); // NRE 방지
    }

    public void SetRunner(CharacterTaskRunner r) => runner = r;

    public void SetRestedThisTurn(bool v) => restedThisTurn = v;

    public void ClearCompleted()
    {
        completed.Clear();
        restedThisTurn = false;                          
    }

    public bool CanDo(FacilityAction act)
    {
        if (string.IsNullOrEmpty(act.id)) return false;
        if (restedThisTurn) return false;                 // ← 휴식하면 그날 끝!
        if (!human) human = GetComponent<Human>();
        if (!human) return false;
        if (act.cost > 0 && human.currentStamina < act.cost) return false;
        if (completed.Contains(act.id)) return false;     // ← 종류별 1회 제한
        return true;
    }

    public void DoAction(FacilityAction act)
    {
        if (string.IsNullOrEmpty(act.id)) return;
        if (completed.Contains(act.id)) return;
        if (restedThisTurn) return;                       // 안전 가드

        if (!human || !human.humanData) return;

        if (act.cost < 0) human.RecoverStamina(-act.cost);
        else if (act.cost > 0)
        {
            if (human.currentStamina < act.cost) return;
            human.ConsumeStamina(act.cost);
        }

        if (act.id == "shelter_rest") restedThisTurn = true; // ← 휴식 잠금

        completed.Add(act.id);
        // (필요 시) runner?.RecalcActionable();
    }

    public bool HasAnyAvailable(IEnumerable<FacilityAction> acts)
    {
        if (acts == null) return false;
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
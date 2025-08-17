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
        // 오늘 휴식했다면, 휴식 외 액션은 금지
        if (restedThisTurn) return false;

        // 인물 캐시가 비어있을 경우 재확보 (NRE 방지)
        if (!human) human = GetComponent<Human>();
        if (!human) return false;

        // 스태미너 체크(소모가 있을 때)
        if (act.cost > 0 && human.currentStamina < act.cost) return false;

        return true;
    }

    public void DoAction(FacilityAction act)
    {
        if (string.IsNullOrEmpty(act.id)) return;
        if (completed.Contains(act.id) || restedThisTurn) return;

        if (restedThisTurn && act.id != "shelter_rest") return;

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
        else if (act.cost > 0)
        {
            if (human.currentStamina < act.cost) return; // 안전 검사
            human.ConsumeStamina(act.cost);
        }

        if (act.id == "shelter_rest")
            restedThisTurn = true;

        completed.Add(act.id);
        runner?.RecalcActionable();
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
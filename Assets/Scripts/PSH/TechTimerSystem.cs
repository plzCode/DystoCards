using System;
using System.Collections.Generic;
using UnityEngine;

public class TechTimerSystem : MonoBehaviour
{
    public static TechTimerSystem Instance { get; private set; }

    // TechCardData(SO) 기준 관리: 카드 오브젝트 파괴와 무관하게 안정적
    private readonly Dictionary<TechCardData, int> remaining = new();
    private readonly HashSet<TechCardData> active = new();

    // 하루 1회 감소 보장용
    private int lastCountedDay = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        if (TurnManager.Instance != null)
            TurnManager.Instance.OnTurnPhaseChanged += OnPhaseChanged;
    }

    private void OnDisable()
    {
        if (TurnManager.Instance != null)
            TurnManager.Instance.OnTurnPhaseChanged -= OnPhaseChanged;
    }

    // “행위 발생 시” 호출: 등록만 한다 (감소 X)
    public void StartCountdown(TechCardData tech, int days)
    {
        if (tech == null)
        {
            Debug.LogWarning("[TechTimer] tech == null, 등록 스킵");
            return;
        }

        if (days <= 0) days = 1; // 안전 가드

        if (!remaining.ContainsKey(tech))
            remaining[tech] = days;

        active.Add(tech);
        Debug.Log($"[TechTimer] 등록: {tech.name}({remaining[tech]}일)");
    }

    // “다음 날 시작”으로 삼을 페이즈를 명확히 정해 TickDay()를 호출
    private void OnPhaseChanged(TurnPhase phase)
    {
        // 프로젝트 규칙에 맞게 ‘새 날’로 보는 페이즈로 바꿔도 됨 (EventDraw 등)
        if (phase == TurnPhase.ExploreAction)
            TickDay();
    }

    // 하루 1회만 감소
    public void TickDay()
    {
        int today = (TurnManager.Instance != null) ? TurnManager.Instance.TurnCount : -1;
        if (today == lastCountedDay) return; // 같은 날 중복 방지
        lastCountedDay = today;

        DecreaseAllOncePerDay();
    }

    private void DecreaseAllOncePerDay()
    {
        if (active.Count == 0) return;

        var toUnlock = new List<TechCardData>();
        var stillActive = new List<TechCardData>();

        foreach (var tech in active)
        {
            int left = remaining.TryGetValue(tech, out var v) ? v : 0;
            left = Mathf.Max(0, left - 1);
            remaining[tech] = left;

            Debug.Log($"[TechTimer] {tech.name} 감소 → {left}");

            if (left <= 0) toUnlock.Add(tech);
            else stillActive.Add(tech);
        }

        // 감소 루프 종료 후 일괄 해금 (순회 중 컬렉션 수정 충돌 방지)
        foreach (var tech in toUnlock)
        {
            UnlockRecipeSafe(tech);
            remaining.Remove(tech);
        }

        // active 재빌드
        active.Clear();
        foreach (var tech in stillActive) active.Add(tech);

        Debug.Log($"[TechTimer] TICK DAY 완료. 남은 진행 항목: {active.Count}");
    }

    private bool UnlockRecipeSafe(TechCardData tech)
    {
        if (tech == null || tech.unlockRecipe == null) return false;

        var cm = CombinationManager.Instance;
        if (cm == null)
        {
            Debug.LogWarning("[TechTimer] CombinationManager.Instance == null, 해금 스킵");
            return false;
        }

        if (cm.HasRecipe(tech.unlockRecipe)) return false;

        cm.RecipesInternalAdd(tech.unlockRecipe);

        if (GradeRecorder.Instance != null)
            GradeRecorder.Instance.recipeOpenCount++;

        Debug.Log($"[TechTimer] 해금 완료: {tech.unlockRecipe.cardName}");
        return true;
    }

    public int GetRemaining(TechCardData tech)
        => (tech != null && remaining.TryGetValue(tech, out var v)) ? v : 0;
}

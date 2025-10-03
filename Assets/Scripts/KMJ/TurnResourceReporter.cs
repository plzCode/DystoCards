using System.Collections.Generic;
using UnityEngine;

public class TurnResourceReporter : MonoBehaviour
{
    private readonly Dictionary<ResourceType, int> delta = new();

    private void Start()
    {
        // ResourceManager.Add 를 대신 호출하도록 래퍼 등록
        ResourceBridge.OnAdd = (type, amount) =>
        {
            if (!delta.ContainsKey(type)) delta[type] = 0;
            delta[type] += amount;
            ResourceManager.Add(type, amount);
            Debug.Log($"[Res] {type} +{amount} (턴 누적)");
        };

        // DayEnd 리포트
        TurnManager.Instance.RegisterPhaseAction(
            TurnPhase.DayEnd, ReportAndClear);
    }

    private void ReportAndClear()
    {
        Debug.Log("<color=cyan>=== DayEnd Report ===</color>");
        foreach (var kv in delta)
            Debug.Log($"{kv.Key} +{kv.Value}");
        delta.Clear();
    }
}

/* ---------- 간단 브리지 ---------- */
public static class ResourceBridge
{
    public static System.Action<ResourceType, int> OnAdd = ResourceManager.Add;
}

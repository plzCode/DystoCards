using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ExpandReward
{
    public string id;     // 카드 ID (예: "001","002","021","025")
    public int count = 1; // 지급 개수
}

public class CleanProgressTracker : MonoBehaviour
{
    public static CleanProgressTracker Instance { get; private set; }
    void Awake() { if (Instance == null) Instance = this; }

    [Header("청소 → 확장 규칙")]
    public int cleansPerExpand = 3;                 // 청소 N번마다
    public List<ExpandReward> rewardsOnExpand = new(); // 확장 시 지급할 보상(ID)

    int cleanCount = 0;

    /// <summary>청소 미니게임 "성공" 시 ActionMenu에서 한 줄로 호출</summary>
    public void RegisterClean()
    {
        cleanCount++;
        Debug.Log($"[CleanProgress] Clean {cleanCount}/{cleansPerExpand}");

        if (cleanCount < cleansPerExpand) return;
        cleanCount = 0;

        // 현재 Frontier 타일 찾기 (MapManager 공식의 '다음 확장 후보')
        var frontier = FindFrontierTile();
        if (frontier == null)
        {
            Debug.Log("[CleanProgress] Frontier 없음(확장 완료 상태?)");
            return;
        }

        // 공식 확장 호출: 비용/해금/다음 프론티어 갱신은 MapManager가 수행
        MapManager.Instance.TryUnlock(frontier); // ← 공식 경로 사용  :contentReference[oaicite:1]{index=1}

        // 확장 성공 확인: Frontier → Unlocked로 상태가 바뀌었는지 체크
        if (frontier.Current == TilePhase.Unlocked)
        {
            // 확장 "시점"에 보상 지급(ID 기준)
            foreach (var r in rewardsOnExpand)
            {
                if (string.IsNullOrEmpty(r.id) || r.count <= 0) continue;
                for (int i = 0; i < r.count; i++)
                {
                    CardManager.Instance.SpawnCardById(r.id, frontier.transform.position);
                    ResourceBank.Instance?.Add(r.id, 1);
                    DaySummary.Instance?.Add(r.id, 1);
                }
            }
        }
    }

    private TileState FindFrontierTile()
    {
        // MapManager 하위 타일 중 Frontier 검색 (공식에서 지정된 현재 후보)  :contentReference[oaicite:2]{index=2}
        var tiles = MapManager.Instance.GetComponentsInChildren<TileState>(true);
        foreach (var t in tiles)
            if (t.Current == TilePhase.Frontier) return t;
        return null;
    }
}
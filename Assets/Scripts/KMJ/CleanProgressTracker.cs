using UnityEngine;

/// <summary>
/// 청소 누적 → 일정 횟수마다 프런티어 해금 + 보상 카드 스폰
/// </summary>
public class CleanProgressTracker : MonoBehaviour
{
    public static CleanProgressTracker Instance { get; private set; }

    [Header("Clean → Unlock")]
    [SerializeField] private int cleansPerUnlock = 3;     // 청소 N회 당 확장

    [Header("Expansion Reward (Card IDs)")]
    [Tooltip("목재 카드 ID")]
    [SerializeField] private string woodCardId = "001";
    [SerializeField] private int woodAmount = 3;

    [Tooltip("석재 카드 ID")]
    [SerializeField] private string stoneCardId = "002";
    [SerializeField] private int stoneAmount = 3;

    [Header("Drop")]
    [Tooltip("보상 카드를 떨어뜨릴 기준 위치(비면 화면 중앙 근처)")]
    [SerializeField] private Transform rewardDropAnchor;

    private int cleanCountThisCycle = 0;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>청소 1회 기록. N회에 도달하면 해금 + 보상 지급</summary>
    public void RegisterClean()
    {
        cleanCountThisCycle++;
        Debug.Log($"[Clean] 진행 {cleanCountThisCycle}/{cleansPerUnlock}");

        if (cleanCountThisCycle < cleansPerUnlock) return;

        var mm = MapManager.Instance;
        if (mm == null) { Debug.LogWarning("[Clean] MapManager 없음"); return; }

        var frontier = mm.GetCurrentFrontierTile();   // MapManager에 추가했던 getter
        if (frontier == null)
        {
            Debug.Log("[Clean] 해금할 Frontier 없음");
            cleanCountThisCycle = 0;                  // 그래도 초기화는 해둠(원하면 유지해도 됨)
            return;
        }

        // 1) 프런티어 해금 (void)
        mm.UnlockFrontierFromClean(frontier);

        // 2) 보상 카드 스폰
        GrantCard(woodCardId, woodAmount);
        GrantCard(stoneCardId, stoneAmount);

        // 4) 카운터 리셋
        cleanCountThisCycle = 0;
    }

    // ---- Helpers ------------------------------------------------------------

    private void GrantCard(string cardId, int count)
    {
        var cm = CardManager.Instance;
        if (cm == null)
        {
            Debug.LogWarning("[CleanReward] CardManager 없음");
            return;
        }

        // 드롭 시작 좌표
        Vector3 pos = rewardDropAnchor ? rewardDropAnchor.position : GetDefaultDropPos();

        for (int i = 0; i < count; i++)
        {
            var spawned = cm.SpawnCardById(cardId, pos);
            if (spawned == null)
            {
                Debug.LogWarning($"[CleanReward] 스폰 실패 (id={cardId})");
            }
            // 보상 카드가 겹치지 않도록 살짝씩 옆으로 흩뿌림
            pos += new Vector3(0.35f, 0f, 0f);
        }
    }

    private Vector3 GetDefaultDropPos()
    {
        var cam = Camera.main;
        if (cam != null)
        {
            // Z는 카메라의 카드 레이어 깊이에 맞춰 조정(씬에 따라 5~12 정도가 안전)
            var v = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.55f, 10f));
            return v;
        }
        return Vector3.zero;
    }

    private string IdToName(string id)
    {
        // 최소 매핑(필요에 맞춰 확장)
        switch (id)
        {
            case "001": return "Wood";
            case "002": return "Stone";
            case "021": return "Potato";
            case "025": return "Carrot";
            default: return id;
        }
    }
}
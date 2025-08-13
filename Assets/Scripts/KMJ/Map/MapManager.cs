using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [Header("Background (Temp_map) — SpriteRenderer")]
    public SpriteRenderer background;   // Pivot = Top‑Left

    [Header("Grid Size (타일 수)")]
    public int width = 18;
    public int height = 10;

    [Header("Initial Unlocked")]
    public int initWidth = 16;
    public int initHeight = 8;

    [Header("Tile Prefab (TileState)")]
    public TileState tilePrefab;

    [Header("Resource")]
    public int costPerTile = 1;
    public int currentResource = 10;

    /* ───────── 내부 ───────── */
    private TileState[,] grid;
    private List<Vector2Int> spiralOrder = new(); // Frontier 진행 순서
    private int cursor = 0;                       // spiralOrder 인덱스

    /* ───────── 초기화 ───────── */
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        InitGrid();
    }

    private void InitGrid()
    {
        if (!background) { Debug.LogError("background 연결 필요"); return; }
        if (!tilePrefab) { Debug.LogError("[Map] tilePrefab is not assigned."); return; }

        /* 1) 셀 크기 & 원점 */
        Vector2 tileSize = new(
            background.bounds.size.x / width,
            background.bounds.size.y / height);
        Vector2 origin = background.transform.position;   // TL

        grid = new TileState[width, height];

        /* 2) 타일 생성 → Locked + 스케일 */
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 pos = new(
                    origin.x + (x + 0.5f) * tileSize.x,
                    origin.y - (y + 0.5f) * tileSize.y,
                    0f);

                var t = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                t.name = $"Tile_{x}_{y}";
                t.transform.localScale = new Vector3(tileSize.x, tileSize.y, 1f);
                t.SetState(TilePhase.Locked);
                grid[x, y] = t;
            }
        }

        /* 3) 중앙 10×6 Unlocked */
        int sx = (width - initWidth) / 2;  // 4
        int sy = (height - initHeight) / 2;  // 2
        int ex = sx + initWidth - 1;        // 13
        int ey = sy + initHeight - 1;        // 7

        for (int y = sy; y <= ey; y++)
            for (int x = sx; x <= ex; x++)
                grid[x, y].SetState(TilePhase.Unlocked);

        /* 4) spiralOrder 계산 (시계방향, 한 겹씩) */
        BuildSpiralOrder(sx, sy, ex, ey);

        /* 5) 첫 Frontier 지정 */
        SetNextFrontier();
    }

    /* ───────── 클릭 → 해금 & 다음 Frontier ───────── */
    public void TryUnlock(TileState tile)
    {
        // 자원이 부족할 때
        if (tile == null) return;

        if (currentResource < costPerTile)
        {
            Debug.Log($"자원 부족! ({currentResource}/{costPerTile}) — {tile.name} 확장 실패");
            return;
        }

        // 확장
        currentResource -= costPerTile;
        tile.SetState(TilePhase.Unlocked);
        Debug.Log($"확장 완료: {tile.name} — 남은 자원 {currentResource}");

        SetNextFrontier();
    }

    /* ───────── spiralOrder & Frontier 지정 ───────── */

    /// <summary>중앙 사각형 경계(sx,sy)-(ex,ey)에서 시작해
    ///  시계방향 나선 순으로 모든 타일 좌표를 spiralOrder에 저장</summary>
    private void BuildSpiralOrder(int sx, int sy, int ex, int ey)
    {
        spiralOrder.Clear();
        cursor = 0;

        int minX = 0, minY = 0;
        int maxX = width - 1, maxY = height - 1;

        // 현재 사각형 경계를 역순으로 넓혀가며 나선 추가
        while (true)
        {
            // 오른쪽으로 (Top Edge)
            for (int x = sx; x <= ex + 1 && x <= maxX; x++)
                AddIfLocked(x, sy);

            // 아래로
            for (int y = sy + 1; y <= ey + 1 && y <= maxY; y++)
                AddIfLocked(ex + 1, y);

            // 왼쪽으로
            for (int x = ex; x >= sx - 1 && x >= minX; x--)
                AddIfLocked(x, ey + 1);

            // 위로
            for (int y = ey; y >= sy - 1 && y >= minY; y--)
                AddIfLocked(sx - 1, y);

            // 다음 외곽으로 확장
            sx--; sy--; ex++; ey++;
            if (sx < minX && sy < minY && ex > maxX && ey > maxY) break;
        }
    }

    private void AddIfLocked(int x, int y)
    {
        if (!InBounds(x, y)) return;
        if (grid[x, y].Current == TilePhase.Locked)
            spiralOrder.Add(new Vector2Int(x, y));
    }

    private void SetNextFrontier()
    {
        // 이전 Frontier가 Unlock된 뒤 호출
        while (cursor < spiralOrder.Count)
        {
            Vector2Int p = spiralOrder[cursor++];
            TileState t = grid[p.x, p.y];
            if (t.Current == TilePhase.Locked)
            {
                t.SetState(TilePhase.Frontier);
                return;
            }
        }
        // cursor가 끝까지 갔으면 더 이상 Frontier 없음 (맵 완전 해금)
        Debug.Log("모든 맵이 확장되었습니다!");
    }

    /* ───────── Tile/World utilities (pivot-agnostic) ───────── */

    // bounds 기반 크기/원점(좌상단) — 스프라이트 Pivot이 어디든 동일 결과
    private Vector2 TileSize => new(
        background.bounds.size.x / width,
        background.bounds.size.y / height);

    private Vector2 OriginTL => new( // 좌상단
        background.bounds.min.x,
        background.bounds.max.y);

    public bool WorldToCell(Vector3 world, out int x, out int y)
    {
        var s = TileSize; var o = OriginTL;
        x = Mathf.FloorToInt((world.x - o.x) / s.x);
        y = Mathf.FloorToInt((o.y - world.y) / s.y); // y 반전
        return InBounds(x, y);
    }

    public Vector3 CellCenter(int x, int y)
    {
        var s = TileSize; var o = OriginTL;
        return new Vector3(o.x + (x + 0.5f) * s.x, o.y - (y + 0.5f) * s.y, 0f);
    }

    public Rect GetCellWorldRect(int x, int y)
    {
        var s = TileSize; var o = OriginTL;
        float xMin = o.x + x * s.x;
        float yMax = o.y - y * s.y;
        return Rect.MinMaxRect(xMin, yMax - s.y, xMin + s.x, yMax);
    }

    public bool IsUnlockedCell(int x, int y)
        => InBounds(x, y) && grid[x, y].Current == TilePhase.Unlocked;

    public bool TrySnapToUnlocked(Vector3 world, out Vector3 snapped)
    {
        if (WorldToCell(world, out int x, out int y) && IsUnlockedCell(x, y))
        {
            snapped = CellCenter(x, y);
            return true;
        }
        snapped = default;
        return false;
    }

    /// <summary>
    /// 카드의 월드 바운즈가 겹치는 **모든** 셀이 Unlocked 인지 검사.
    /// 검은(잠김) 타일과 1픽셀이라도 겹치면 false.
    /// </summary>
    public bool AreAllCellsUnlocked(Bounds b)
    {
        var s = TileSize; var o = OriginTL;
        const float eps = 1e-4f;

        int x0 = Mathf.FloorToInt((b.min.x - o.x) / s.x);
        int x1 = Mathf.FloorToInt(((b.max.x - eps) - o.x) / s.x);
        int y0 = Mathf.FloorToInt((o.y - (b.max.y - eps)) / s.y);
        int y1 = Mathf.FloorToInt((o.y - b.min.y) / s.y);

        for (int y = y0; y <= y1; y++)
        {
            for (int x = x0; x <= x1; x++)
            {
                if (!InBounds(x, y) || grid[x, y].Current != TilePhase.Unlocked)
                    return false;
            }
        }
        return true;
    }

    /* ───────── Helpers ───────── */
    private bool InBounds(int x, int y)
        => x >= 0 && x < width && y >= 0 && y < height;

    // (선택) 외부가 현재 타일 상태를 조회할 때 쓰는 getter
    public TileState GetTile(int x, int y) => InBounds(x, y) ? grid[x, y] : null;
}
using UnityEngine;

public class CardTileBarrier : MonoBehaviour
{
    Vector3 lastSafe;

    void OnEnable() => lastSafe = transform.position;

    void Update()
    {
        var m = MapManager.Instance; if (!m) return;
        var b = GetWorldBounds();

        if (m.AreAllCellsUnlocked(b))
            lastSafe = transform.position;   // 안쪽: 자유 이동
        else
            transform.position = lastSafe;   // 바깥: 즉시 되돌림(벽)
    }

    Bounds GetWorldBounds()
    {
        var rs = GetComponentsInChildren<Renderer>(true);
        if (rs.Length == 0) return new Bounds(transform.position, Vector3.zero);
        var b = rs[0].bounds;
        for (int i = 1; i < rs.Length; i++) b.Encapsulate(rs[i].bounds);
        return b;
    }
}
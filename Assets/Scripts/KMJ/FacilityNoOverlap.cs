using System.Collections.Generic;
using UnityEngine;

/// 시설 카드 간 중첩 방지: 겹치면 즉시 이전 안전 위치로 복귀
[DisallowMultipleComponent]
public class FacilityNoOverlap : MonoBehaviour
{
    static readonly List<FacilityNoOverlap> all = new();
    Vector3 lastSafe;

    void OnEnable() { all.Add(this); lastSafe = transform.position; }
    void OnDisable() { all.Remove(this); }

    void Update()
    {
        if (IsOverlappingAnyInternal())
        {
            transform.position = lastSafe; // 겹치면 복귀
        }
        else
        {
            lastSafe = transform.position; // 안전지점 갱신
        }
    }

    bool IsOverlappingAnyInternal()
    {
        var myB = GetBounds();
        foreach (var f in all)
        {
            if (f == this) continue;
            if (myB.Intersects(f.GetBounds())) return true;
        }
        return false;
    }

    Bounds GetBounds()
    {
        var rs = GetComponentsInChildren<Renderer>(true);
        if (rs.Length == 0) return new Bounds(transform.position, Vector3.zero);
        var b = rs[0].bounds;
        for (int i = 1; i < rs.Length; i++) b.Encapsulate(rs[i].bounds);
        return b;
    }

    // ActionMenu 쪽에서 사용할 공개 확인용
    public bool IsOverlappingAny() => IsOverlappingAnyInternal();
}
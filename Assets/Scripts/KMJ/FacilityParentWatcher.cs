using UnityEngine;

[RequireComponent(typeof(CharacterCard2D))]
public class FacilityParentWatcher : MonoBehaviour
{
    CharacterCard2D chr;
    Transform lastParent;

    void Awake()
    {
        chr = GetComponent<CharacterCard2D>();
        lastParent = transform.parent;
    }

    void Update()
    {
        if (transform.parent == lastParent) return;   // 부모 안 바뀜
        lastParent = transform.parent;                // 변경 기록

        // 부모가 존재하고 Card2D가 붙어 있는지 먼저 확인
        if (lastParent != null &&
            lastParent.TryGetComponent(out Card2D facCard) &&
            facCard.cardData is FacilityCardData facData)
        {
            // 겹침 검사: 겹쳐져 있으면 메뉴 열지 않음
            if (lastParent.TryGetComponent<FacilityNoOverlap>(out var noOL) && noOL.IsOverlappingAny())
                return;

            Debug.Log($"[Watcher] ① parent={lastParent.name} ② facCard OK " +
              $"③ ActionMenu.Instance={(ActionMenu.Instance == null ? "NULL" : "OK")}");

            // 메뉴 열기
            ActionMenu.Instance.Open(facData.facilityType, chr);
            Debug.Log($"[ParentWatcher] {chr.name} → {facData.facilityType}, 메뉴 오픈");
        }
    }
}


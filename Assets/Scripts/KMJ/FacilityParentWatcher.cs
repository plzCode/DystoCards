using UnityEngine;

[RequireComponent(typeof(CharacterCard2D))]
public class FacilityParentWatcher : MonoBehaviour
{
    [Tooltip("메뉴가 튀는 걸 막기 위한 유예시간(초)")]
    [SerializeField] float detachGrace = 0.08f;

    CharacterCard2D chr;        // 이 스크립트가 붙은 캐릭터
    Transform lastParent;       // 직전 부모(시설 or 기타)
    Card2D lastFacility;     // 직전에 올라와 있던 시설 카드
    float graceTimer;       // 유예 타이머

    void Awake()
    {
        chr = GetComponent<CharacterCard2D>();
        lastParent = transform.parent;
        lastFacility = GetFacilityFrom(lastParent);
    }

    void Update()
    {
        // 1) 부모 변경 감지 → 유예 타이머 리셋
        if (transform.parent != lastParent)
        {
            lastParent = transform.parent;
            graceTimer = detachGrace;
        }

        // 2) 현재 부모가 시설인지 판정
        Card2D nowFacility = GetFacilityFrom(transform.parent);

        // 2-1) 시설 위에 **있는 경우**: 메뉴 열기(시설이 바뀌었을 때만)
        if (nowFacility != null)
        {
            // 겹침 방지: 시설 카드가 겹쳐 있으면 메뉴 열지 않음
            if (nowFacility.TryGetComponent<FacilityNoOverlap>(out var noOL) && noOL.IsOverlappingAny())
                return;

            // 시설이 새로 바뀌었을 때만 오픈(중복 호출 방지)
            if (nowFacility != lastFacility)
            {
                if (nowFacility.cardData is FacilityCardData facData)
                {
                    ActionMenu.Instance?.Open(facData.facilityType, chr);
                }
                lastFacility = nowFacility;
            }
            return;
        }

        // 2-2) 시설 위에 **없는 경우**: 잠깐의 유예 뒤 메뉴 닫기
        if (graceTimer > 0f)
        {
            graceTimer -= Time.deltaTime;
            return;
        }

        if (lastFacility != null)
        {
            lastFacility = null;
            ActionMenu.Instance?.Close();   // 사람이 떠났으니 닫기
        }
    }

    // 주어진 트랜스폼이 시설 카드인지 판정해서 Card2D 반환
    Card2D GetFacilityFrom(Transform parent)
    {
        if (!parent) return null;
        if (!parent.TryGetComponent(out Card2D c2d)) return null;
        return (c2d.cardData is FacilityCardData) ? c2d : null;
    }
}

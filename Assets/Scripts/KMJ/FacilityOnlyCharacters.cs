using UnityEngine;

[RequireComponent(typeof(FacilityParentWatcher))]
public class FacilityOnlyCharacters : MonoBehaviour
{
    FacilityParentWatcher watcher;

    void Awake() => watcher = GetComponent<FacilityParentWatcher>();

    // FacilityParentWatcher가 “시설 위에 있는 최상단 카드”를 갱신할 때마다 호출되도록
    // watcher 쪽에서 아래 함수를 한 줄 호출해 주세요.
    public void Enforce(Card2D topCardOnFacility)
    {
        if (!topCardOnFacility) return;

        // 1) 캐릭터 카드인지 검사
        var isCharacter = topCardOnFacility.TryGetComponent<CharacterCard2D>(out _);

        if (!isCharacter)
        {
            // 2) 캐릭터가 아니면 즉시 분리/반환
            //   - 카드 시스템에 “부모 해제/원래 위치 복귀” 유틸이 있다면 그걸 호출
            //   - 없다면 살짝 밀어내서 겹침이 안 되게만 처리
            topCardOnFacility.transform.position += new Vector3(0, 0.6f, 0);
        }
    }
}

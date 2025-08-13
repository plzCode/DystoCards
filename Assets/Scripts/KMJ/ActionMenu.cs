using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionMenu : MonoBehaviour
{
    public static ActionMenu Instance { get; private set; }
    void Awake() => Instance = this;

    [Header("Menu Panel & List")]
    [SerializeField] private GameObject panel;       // 메뉴 패널 (켜고/끄는 용도)
    [SerializeField] private Transform listRoot;     // 버튼들이 들어갈 부모
    [SerializeField] private Button btnPrefab;       // 공용 버튼 프리팹

    [Header("Mini-Game (UI Overlay)")]
    [SerializeField] private Transform miniGameRoot; // Canvas 밑 풀스크린 패널(입력 차단)
    [SerializeField] private GameObject pfMini_Harvest;
    [SerializeField] private GameObject pfMini_Clean;
    [SerializeField] private GameObject pfMini_Felling;
    [SerializeField] private GameObject pfMini_Mining;

    // 현재 컨텍스트
    private CharacterCard2D currentChar;
    private FacilityType currentFacility;

    // 메뉴 열기: FacilityParentWatcher에서 호출
    public void Open(FacilityType facility, CharacterCard2D chr)
    {
        currentFacility = facility;      // ★ 현재 시설 타입 저장
        currentChar = chr;

        ClearButtons();

        // ActionLibrary에서 액션 목록 받아 버튼 구성
        var actions = ActionLibrary.GetActions(facility);
        foreach (var act in actions)
        {
            if (!chr.CanDo(act)) continue;

            var b = Instantiate(btnPrefab, listRoot);
            // Text 설정 (TMP 또는 기본 Text 자동 대응)
            var tmp = b.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp) tmp.text = act.label;
            else
            {
                var t = b.GetComponentInChildren<Text>(true);
                if (t) t.text = act.label;
            }

            b.onClick.AddListener(() => OnClickAction(act));
        }

        if (listRoot.childCount > 0)
            UIManager.Instance.TogglePanel(panel);  // 메뉴 열기
    }

    public void Close() => UIManager.Instance.TogglePanel(panel);

    private void ClearButtons()
    {
        for (int i = listRoot.childCount - 1; i >= 0; i--)
            Destroy(listRoot.GetChild(i).gameObject);
    }

    // 버튼 클릭 → 즉시 처리(Rest) or UI 미니게임 실행
    private void OnClickAction(FacilityAction act)
    {
        if (currentChar == null)
        {
            Debug.LogError("[ActionMenu] currentChar is null");
            Close();
            return;
        }

        // 어떤 미니게임 프리팹을 쓸지 결정 (없으면 즉시 처리)
        GameObject prefab = PrefabFor(currentFacility, act.id);

        // 1) 프리팹 없음 → 즉시 처리(휴식 등)
        if (prefab == null)
        {
            // 스태미너 처리/중복 방지/휴식-당일잠금은 CharacterCard2D.DoAction에서 처리
            currentChar.DoAction(act);

            // (기존 보상 지급 방식 유지: 카드 스폰)
            if (act.rewards != null)
            {
                foreach (var r in act.rewards)
                {
                    if (string.IsNullOrEmpty(r.idOrName) || r.count <= 0) continue;
                    for (int i = 0; i < r.count; i++)
                    {
                        if (r.useId)
                            CardManager.Instance.SpawnCardById(r.idOrName, currentChar.transform.position);
                        else
                            CardManager.Instance.SpawnCardByName(r.idOrName, currentChar.transform.position);
                    }
                }
            }

            CharacterTaskRunner.Instance?.RecalcActionable();
            Close();
            return;
        }

        // 2) 프리팹 있음 → 캔버스 미니게임 실행
        Close(); // 메뉴 닫기

        // MinigameUIRunner가 TurnBridge.BeginAction()/MarkComplete()까지 처리
        MinigameUIRunner.Show(prefab, miniGameRoot, success =>
        {
            if (success)
            {
                // 보상 지급(카드 스폰 방식을 그대로 사용)
                if (act.rewards != null)
                {
                    foreach (var r in act.rewards)
                    {
                        if (string.IsNullOrEmpty(r.idOrName) || r.count <= 0) continue;
                        for (int i = 0; i < r.count; i++)
                        {
                            if (r.useId)
                                CardManager.Instance.SpawnCardById(r.idOrName, currentChar.transform.position);
                            else
                                CardManager.Instance.SpawnCardByName(r.idOrName, currentChar.transform.position);
                        }
                    }
                }

                // 스태미너 소비/완료 처리(휴식 아님)
                currentChar.DoAction(act);
                CharacterTaskRunner.Instance?.RecalcActionable();
            }
            else
            {
                Debug.Log("[MiniUI] 실패 → 보상/소모 없음");
            }
        });
    }

    // 액션ID → 프리팹 매핑 (ActionLibrary의 id에 맞춰 수정)
    private GameObject PrefabFor(FacilityType type, string actionId)
    {
        switch (type)
        {
            case FacilityType.Farm:        // 수확
                return actionId == "farm_harvest" ? pfMini_Harvest : null;

            case FacilityType.Shelter:     // 청소, 휴식(즉시)
                if (actionId == "shelter_clean") return pfMini_Clean;
                if (actionId == "shelter_rest") return null; // 즉시 처리
                return null;

            case FacilityType.ForestMine:  // 벌목/채광
                if (actionId == "forest_felling") return pfMini_Felling;
                if (actionId == "forest_mine") return pfMini_Mining;
                return null;
        }
        return null;
    }
}

/* ---- 별도 라이브러리 ---- */
public static class ActionLibrary
{
    private static readonly Dictionary<FacilityType, FacilityAction[]> map =
        new()
        {
            [FacilityType.Farm] = new[] {
                new FacilityAction { id="farm_harvest", label="Harvest", cost=1, rewards = new [] {new CardReward{ useId=false, idOrName="Food",  count=1 } }
                },
            },
            [FacilityType.Shelter] = new[] {
                new FacilityAction { id="shelter_rest", label="Rest", cost=-2, rewards = new CardReward[0] },
                new FacilityAction { id="shelter_clean", label="Clean", cost=1, rewards = new [] {
                    new CardReward{ useId=false, idOrName="Wood",  count=1 },
                    new CardReward{ useId=false, idOrName="Stone", count=1 } }
                },
            },
            [FacilityType.ForestMine] = new[] {
                new FacilityAction { id="forest_felling", label="Felling", cost=1, rewards = new [] { new CardReward{ useId=false, idOrName="Wood",  count=2 } }
                },
                new FacilityAction { id="forest_mine", label="Mining", cost=1, rewards = new [] { new CardReward{ useId=false, idOrName="Stone", count=2 } }
                },
            },
        };

    public static IEnumerable<FacilityAction> GetActions(FacilityType f) => map[f];
}

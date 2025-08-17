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
    [SerializeField] private GameObject pfMini_Rest;

    [Header("Harvest Settings")]
    [SerializeField] private string seedCardId;
    [SerializeField] private bool seedById = true;

    // 현재 컨텍스트
    private CharacterCard2D currentChar;
    private FacilityType currentFacility;

    public bool IsOpen => panel && panel.activeSelf;

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

            // Harvest 버튼 활성 조건: (허용창 true) 또는 (씨앗 보유)
            if (act.id == "farm_harvest")
            {
                if (!HarvestWindow.PermitActive && !HasAnyCardById(seedCardId))
                {
                    // 씨앗 없으면 버튼을 비활성화해서 표시(툴팁은 UI 쪽 처리)
                    var bGray = Instantiate(btnPrefab, listRoot);
                    SetButtonText(bGray, act.label + " (Need Seed)");
                    bGray.interactable = false;
                    continue;
                }
            }

            var b = Instantiate(btnPrefab, listRoot);
            SetButtonText(b, act.label);
            var captured = act;
            b.onClick.AddListener(() => OnClickAction(captured));
        }

        if (listRoot.childCount > 0)
            panel.SetActive(true);  // 메뉴 열기
    }

    public void Close() => panel.SetActive(false);

    private void ClearButtons()
    {
        for (int i = listRoot.childCount - 1; i >= 0; i--)
            Destroy(listRoot.GetChild(i).gameObject);
    }

    private void SetButtonText(Button b, string label)
    {
        var tmp = b.GetComponentInChildren<TextMeshProUGUI>(true);
        if (tmp) tmp.text = label;
        else
        {
            var t = b.GetComponentInChildren<Text>(true);
            if (t) t.text = label;
        }
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

        if (act.id == "farm_harvest")
        {
            Close();

            // 턴 전체 허용이 아니면 씨앗 1장 소모 시도
            if (!HarvestWindow.PermitActive)
            {
                if (!TryConsumeCardById(seedCardId))
                {
                    TurnManager.Instance?.MarkActionComplete(); // 안전: 큐 정지 방지
                    return;
                }
                HarvestWindow.PermitActive = true;
            }

            // 미니게임 여부
            var prefabH = PrefabFor(currentFacility, act.id);
            if (prefabH == null)
            {
                // 즉시 처리: 랜덤 ID로 1개 지급
                GiveHarvestReward1();
                currentChar.DoAction(act);
                TurnManager.Instance?.MarkActionComplete();
                CharacterTaskRunner.Instance?.RecalcActionable();
            }
            else
            {
                MinigameUIRunner.Show(prefabH, miniGameRoot, success =>
                {
                    if (success)
                    {
                        GiveHarvestReward1();
                        currentChar.DoAction(act);
                        CharacterTaskRunner.Instance?.RecalcActionable();
                    }
                    TurnManager.Instance?.MarkActionComplete();
                });
            }
            return;
        }

        // 공통 프리팹 조회
        GameObject prefab = PrefabFor(currentFacility, act.id);
        bool isRest = (act.id == "shelter_rest");
        bool isClean = (act.id == "shelter_clean");

        // === 즉시 처리 ===
        if (prefab == null)
        {
            Close();

            if (isRest)
            {
                ApplyRestEffects(act);
                currentChar.SetRestedThisTurn(true);
            }

            // CLEAN은 자체 보상 없음(확장 시 보상), 성공 시 카운트 등록
            if (isClean)
                CleanProgressTracker.Instance?.RegisterClean();

            currentChar.DoAction(act);

            // 일반 작업 보상 지급 (Rest/Clean 제외)
            if (!isRest && !isClean && act.rewards != null)
                PayoutRewards(act.rewards, currentChar.transform.position);

            TurnManager.Instance?.MarkActionComplete();
            CharacterTaskRunner.Instance?.RecalcActionable();
            return;
        }

        // 2) 프리팹 있음 → 캔버스 미니게임 실행
        Close(); // 메뉴 닫기

        // === 미니게임 처리 ===
        Close();
        MinigameUIRunner.Show(prefab, miniGameRoot, success =>
        {
            if (success)
            {
                if (isRest)
                {
                    ApplyRestEffects(act);
                    currentChar.SetRestedThisTurn(true); // ★ 추가
                }

                if (isClean)
                    CleanProgressTracker.Instance?.RegisterClean();
                else if (act.rewards != null)
                    PayoutRewards(act.rewards, currentChar.transform.position);

                currentChar.DoAction(act);
                CharacterTaskRunner.Instance?.RecalcActionable();
            }
            TurnManager.Instance?.MarkActionComplete();
        });
    }

    // 휴식 효과(+HP1, +Mind2, +Stam2)
    private void ApplyRestEffects(FacilityAction act)
    {
        var human = currentChar.GetComponent<Human>();
        var status = currentChar.GetComponent<Character>();

        if (status) status.Heal(1);                   // HP +1 (내부 클램프)
        if (human) human.RecoverMentalHealth(2);      // Mind +2

        // Rest cost 정책:
        //  - cost >= 0 : DoAction이 회복 안 하므로 여기서 +2
        //  - cost <  0 : DoAction 내부에서 회복 처리되면 스킵
        if (human && act.cost >= 0)
            human.RecoverStamina(2);                  // Stam +2 (내부 클램프)
    }

    // 수확 보상: 감자/당근 5:5, 1개(ID로 지급 + 집계)
    private void GiveHarvestReward1()
    {
        string id = (Random.value < 0.5f) ? "021" : "025";
        CardManager.Instance.SpawnCardById(id, currentChar.transform.position);
        ResourceBank.Instance?.Add(id, 1);
        DaySummary.Instance?.Add(id, 1);
    }

    // 일반 보상 지급(카드 스폰 + 은행/요약 누적)
    private void PayoutRewards(IEnumerable<CardReward> rewards, Vector3 at)
    {
        if (rewards == null) return;

        foreach (var r in rewards)
        {
            if (string.IsNullOrEmpty(r.idOrName) || r.count <= 0) continue;
            for (int i = 0; i < r.count; i++)
            {
                if (r.useId)
                {
                    CardManager.Instance.SpawnCardById(r.idOrName, at);
                    ResourceBank.Instance?.Add(r.idOrName, 1);
                    DaySummary.Instance?.Add(r.idOrName, 1);
                }
                else
                {
                    CardManager.Instance.SpawnCardByName(r.idOrName, at);
                }
            }
        }
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
                if (actionId == "shelter_rest") return pfMini_Rest != null ? pfMini_Rest : null;
                return null;

            case FacilityType.ForestMine:  // 벌목/채광
                if (actionId == "forest_felling") return pfMini_Felling;
                if (actionId == "forest_mine") return pfMini_Mining;
                return null;
        }
        return null;
    }

    // 간단 카드 인벤 스캔/소모(씬 내 카드 1장 버전)
    private bool HasAnyCardById(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;
        foreach (var c in FindObjectsOfType<Card2D>())
            if (c && c.cardData && c.cardData.cardId == id) return true;
        return false;
    }

    private bool TryConsumeCardById(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;
        foreach (var c in FindObjectsOfType<Card2D>())
        {
            if (c && c.cardData && c.cardData.cardId == id)
            {
                CardManager.Instance.DestroyCard(c);
                return true;
            }
        }
        return false;
    }
}

/* ---- 별도 라이브러리 ---- */
public static class ActionLibrary
{
    private static readonly Dictionary<FacilityType, FacilityAction[]> map =
        new()
        {
            [FacilityType.Farm] = new[] {
                // Harvest: 보상은 코드에서 랜덤 지급 → 여기 rewards 비워 중복 방지
                new FacilityAction { id="farm_harvest", label="Harvest", cost=1, rewards = new CardReward[0] },
            },
            [FacilityType.Shelter] = new[] {
                // 🟢 Rest: cost=0 권장(HP/Mind/Stam은 ApplyRestEffects에서 처리)
                new FacilityAction { id="shelter_rest", label="Rest", cost=0, rewards = new CardReward[0] },
                // Clean: 자체 보상 없음(확장 시 보상 지급)
                new FacilityAction { id="shelter_clean", label="Clean", cost=1, rewards = new CardReward[0] },
            },
            [FacilityType.ForestMine] = new[] {
                new FacilityAction { id="forest_felling", label="Felling", cost=1, rewards = new [] { new CardReward{ useId=true, idOrName="001",  count=2 } }
                },
                new FacilityAction { id="forest_mine", label="Mining", cost=1, rewards = new [] { new CardReward{ useId=true, idOrName="002", count=2 } }
                },
            },
        };

    public static IEnumerable<FacilityAction> GetActions(FacilityType f) => map[f];
}

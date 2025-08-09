using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ActionMenu : MonoBehaviour
{
    public static ActionMenu Instance;

    [Header("Refs")]
    [SerializeField] private GameObject panel;       // 전체 패널
    [SerializeField] private Button btnPrefab;       // 버튼 프리팹
    [SerializeField] private Transform listRoot;     // 버튼 부모

    private CharacterCard2D currentChar;

    /* ─────────────────────── */

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Open(FacilityType f, CharacterCard2D chr)
    {
        currentChar = chr;
        ClearButtons();

        foreach (var act in ActionLibrary.GetActions(f))
        {
            if (!chr.CanDo(act)) continue;           // 이미 했거나 스태미너 부족

            var b = Instantiate(btnPrefab, listRoot);
            b.GetComponentInChildren<TextMeshProUGUI>().text = act.label;
            b.onClick.AddListener(() => OnClickAction(act));
        }

        if (listRoot.childCount == 0)
        {
            Debug.Log("선택 가능한 작업이 없습니다");
            return;
        }

        UIManager.Instance.TogglePanel(panel);       // ClickCatcher + 레이어 전환
    }

    private void OnClickAction(FacilityAction act)
    {
        currentChar.DoAction(act);
        ResourceBridge.OnAdd(act.reward, act.amount);

        var stm = currentChar.GetComponent<Human>().currentStamina;
        Debug.Log($" {currentChar.name} {act.label} 완료, Stm {stm}");

        CharacterTaskRunner.Instance.RecalcActionable();
        UIManager.Instance.TogglePanel(panel);       // 패널 닫고 입력 복귀
    }

    private void ClearButtons()
    {
        foreach (Transform c in listRoot) Destroy(c.gameObject);
    }
}

/* ---- 별도 라이브러리 ---- */
public static class ActionLibrary
{
    private static readonly Dictionary<FacilityType, FacilityAction[]> map =
        new()
        {
            [FacilityType.Farm] = new[]
            {
                new FacilityAction{ id="farm_plant", label="Farm", cost=3, reward=ResourceType.Wood, amount=1 }
            },
            [FacilityType.Shelter] = new[]
            {
                new FacilityAction{ id="shelter_rest",  label="Rest", cost=-2, reward=ResourceType.Wood, amount=0 },
                new FacilityAction{ id="shelter_clean", label="Clean", cost=2, reward=ResourceType.Wood, amount=1 },
                new FacilityAction{ id="shelter_research", label="Research", cost=3, reward=ResourceType.Wood, amount=1 }
            },
            [FacilityType.Workbench] = new[]
            {
                new FacilityAction{ id="workbench_craft", label="Craft", cost=4, reward=ResourceType.Wood, amount=1 }
            }
        };

    public static IEnumerable<FacilityAction> GetActions(FacilityType f) => map[f];
}

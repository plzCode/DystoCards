using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 기술 카드 버튼을 제어하는 컴포넌트
/// - 연결된 TechCardData가 해금되었는지 확인
/// - 해금되지 않은 경우 버튼을 통해 해금 처리
/// - 해금 시 해당 레시피도 RecipeBook에 등록됨
/// </summary>
public class TechCheck : MonoBehaviour
{

    [Header("기술 데이터 (ScriptableObject)")]
    [SerializeField] private TechCardData techCard; // 이 버튼이 제어할 기술
    [SerializeField] private RecipeCardData techRecipe; // 기술 해금에 필요한 레시피 (선택적, 필요시 연결)
    [SerializeField] private RecipeCardData recipeCard; // 기술 해금에 필요한 레시피 (선택적, 필요시 연결)
    [SerializeField] private RecipeCardData beforeRecipe; // 기술 해금 전 필요한 레시피 (선택적, 필요시 연결)

    [Header("연결된 오브젝트")]
    //[SerializeField] private GameObject techObject;         // 현재 기술 카드 오브젝트 (파괴 대상)
    //[SerializeField] private GameObject techResultPrefab;   // 해금 후 생성될 프리팹
    [SerializeField] private GameObject techTooltipUI;      // 툴팁 UI
    [SerializeField] private GameObject uiToClose;          // 버튼 누를 시 닫을 UI

    private Button button;
    private Image buttonImage;
    private Text buttonText;

    private bool alreadyDisabled = false;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        buttonText = GetComponentInChildren<Text>();

        if (techCard == null)
        {
            Debug.LogError("[TechCheck] TechCardData가 연결되지 않았습니다.");
            return;
        }

        //RefreshInteractable();
    }

    public void OnTechButton()
    {
        var cm = CombinationManager.Instance;
        if (cm == null) return;
        var unlock = techCard.unlockRecipe;

        bool hasUnlock = (unlock != null) && cm.HasRecipe(unlock);
        bool hasRequired = (recipeCard != null) && cm.HasRecipe(recipeCard);
        bool hasBeforeRecipe = (beforeRecipe != null) && !cm.HasRecipe(beforeRecipe);

        Debug.Log(
            $"[TechCheck] hasUnlock={hasUnlock}, hasRequired={hasRequired}, " +
            $"unlock={(unlock ? unlock.cardName : "null")}, required={(recipeCard ? recipeCard.cardName : "null")}"
        );

        //if(hasBeforeRecipe)
        //{
        //    DisableButtonCompletely();
        //    return;
        //}

        //// 1) 해금 없음 + 필요한 것 있음 -> 버튼 비활성화
        //if (hasUnlock && hasRequired)
        //{
        //    DisableButtonCompletely();
        //    Debug.Log("[TechCheck] → 비활성화 분기 진입");
        //    return;
        //}

        //// 2) 해금 없음 + 필요 없음 -> techRecipe 추가
        //if (!hasUnlock && !hasRequired)
        //{
        //    if (techRecipe == null)
        //    {
        //        Debug.LogWarning("[TechCheck] techRecipe가 지정되지 않았습니다.");
        //        return;
        //    }

        //    if (!cm.HasRecipe(techRecipe))
        //    {
        //        cm.AddRecipeUnique(techRecipe); // CombinationManager에 AddRecipeUnique 메서드 필요
        //        Debug.Log($"[TechCheck] techRecipe 추가: {techRecipe.cardName}");
        //    }

        //    EnableButtonCompletely();
        //    //return;
        //}

        // 3) 나머지 경우 -> 정상 동작 (UI 닫고, 카드 스폰)
        cm.AddRecipeUnique(techRecipe);
        if (uiToClose != null) uiToClose.SetActive(false);
        if (techTooltipUI != null) techTooltipUI.SetActive(false);
        CardManager.Instance.SpawnCardById(techCard.cardId, Vector3.zero);
        //EnableButtonCompletely();
        AudioManager.Instance.PlaySFX("클릭");
        DisableButtonCompletely();
    }



    /// <summary>
    /// 버튼을 완전히 비활성화 (중복 눌림 방지, UI 효과 제거)
    /// </summary>
    private void DisableButtonCompletely()
    {
        if (alreadyDisabled) return;
        alreadyDisabled = true;

        if (button != null) button.interactable = false;
        if (buttonImage != null) buttonImage.raycastTarget = false;
        if (buttonText != null) buttonText.raycastTarget = false;
    }

    private void EnableButtonCompletely()
    {
        alreadyDisabled = false;

        if (button != null) button.interactable = true;
        if (buttonImage != null) buttonImage.raycastTarget = true;
        if (buttonText != null) buttonText.raycastTarget = true;
    }

    private void RefreshInteractable()
    {
        var cm = CombinationManager.Instance;
        if (cm == null || techCard == null) return;

        var unlock = techCard.unlockRecipe;
        bool hasUnlock = (unlock != null) && cm.HasRecipe(unlock);
        bool hasRequired = (recipeCard != null) && cm.HasRecipe(recipeCard);
        bool missingBefore = (beforeRecipe != null) && cm.HasRecipe(beforeRecipe);

        Debug.Log($"hasUnlock={hasUnlock}, " +
                  $"hasRequired={hasRequired}, " +
                  $"missingBefore={missingBefore}, " +
                  $"unlock={(unlock != null ? unlock.cardName : "null")}, " +
                  $"required={(recipeCard != null ? recipeCard.cardName : "null")}, " +
                  $"before={(beforeRecipe != null ? beforeRecipe.cardName : "null")}");

        // 예: 필요한 레시피가 없거나 before가 없으면 비활성화
        bool shouldDisable = hasRequired && hasUnlock;


        //if (missingBefore)
        //{
        //    if (shouldDisable)
        //    {
        //        DisableButtonCompletely();
        //    }
        //    else
        //    {
        //        EnableButtonCompletely();
        //    }
        //}
        //else
        //    DisableButtonCompletely();

        bool shouldDisable1 = unlock && hasRequired;

        if (missingBefore)
        {
            if (!shouldDisable1)
            {
                EnableButtonCompletely();
            }
            else
            {
                DisableButtonCompletely();
            }
        }
        else
            DisableButtonCompletely();

    }


    private void OnEnable()
    {
        RefreshInteractable();   // ★ UI 들어오자마자 판정 → 이미 꺼져있음
    }
}

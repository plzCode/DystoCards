using System;
using UnityEngine;
using UnityEngine.UI;

public class Recipe_Combination_Maker : MonoBehaviour
{

    [SerializeField] private RecipeCardData recipeCard; // 기술 해금에 필요한 레시피 (선택적, 필요시 연결)
    [SerializeField] private ResourceCardData Card; // 조합 카드 

    [SerializeField] private GameObject techTooltipUI;      // 툴팁 UI
    [SerializeField] private GameObject uiToClose;          // 버튼 누를 시 닫을 UI

    private Button button;
    private Image buttonImage;
    private Text buttonText;

    private bool alreadyDisabled = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<Text>();

        //RefreshInteractable();
    }

    public void OnTechButton()
    {
        var cm = CombinationManager.Instance;
        if (cm == null) return;

        bool hasRequired = (recipeCard != null) && cm.HasRecipe(recipeCard);


        // 3) 나머지 경우 -> 정상 동작 (UI 닫고, 카드 스폰)
        if (uiToClose != null) uiToClose.SetActive(false);
        if (techTooltipUI != null) techTooltipUI.SetActive(false);
        CardManager.Instance.SpawnCardById(Card.cardId, Vector3.zero);
        EnableButtonCompletely();
    }

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
        if (cm == null) return;

        bool hasRequired = (recipeCard != null) && cm.HasRecipe(recipeCard);


        bool shouldDisable = !hasRequired;

        if (shouldDisable) DisableButtonCompletely();
        else EnableButtonCompletely();
    }

    private void OnEnable()
    {
        RefreshInteractable();   // ★ UI 들어오자마자 판정 → 이미 꺼져있음
    }
}

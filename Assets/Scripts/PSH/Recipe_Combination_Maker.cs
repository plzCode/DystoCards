using System;
using UnityEngine;
using UnityEngine.UI;

public class Recipe_Combination_Maker : MonoBehaviour
{

    [SerializeField] private RecipeCardData recipeCard; // ��� �رݿ� �ʿ��� ������ (������, �ʿ�� ����)
    [SerializeField] private ResourceCardData Card; // ���� ī�� 

    [SerializeField] private GameObject techTooltipUI;      // ���� UI
    [SerializeField] private GameObject uiToClose;          // ��ư ���� �� ���� UI

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


        // 3) ������ ��� -> ���� ���� (UI �ݰ�, ī�� ����)
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
        RefreshInteractable();   // �� UI �����ڸ��� ���� �� �̹� ��������
    }
}

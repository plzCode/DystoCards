using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HumanIcon : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject staminaBar;
    [SerializeField] private GameObject strengthBar;
    [SerializeField] private GameObject mentalBar; 
    

    [SerializeField] private Button selectButton;
    private HumanCardData humanCardData;
    private ExploreInfo exploreInfoUI;
    private GameObject scrollView;





    public void Setup(HumanCardData _human, ExploreInfo exploreInfo, GameObject parentScrollView)
    {
        humanCardData = _human;
        exploreInfoUI = exploreInfo;
        scrollView = parentScrollView;

        // UI ¼¼ÆÃ
        if (iconImage != null) iconImage.sprite = _human.cardImage;
        if (nameText != null) nameText.text = _human.cardName;

        UIBarUtility.SetBarColor(staminaBar, (int)_human.stamina, UIBarUtility.StaminaColor);
        UIBarUtility.SetBarColor(strengthBar, (int)_human.attack_power, UIBarUtility.StrengthColor);
        UIBarUtility.SetBarColor(mentalBar, (int)_human.consume_hunger, UIBarUtility.WarningColor);

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() =>
        {
            exploreInfoUI.SetHumanInfo(humanCardData);
            scrollView.SetActive(false);
        });
    }
}

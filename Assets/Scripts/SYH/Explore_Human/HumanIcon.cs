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





    public void Setup(Human _human, ExploreInfo exploreInfo, GameObject parentScrollView)
    {
        humanCardData = _human.humanData;
        exploreInfoUI = exploreInfo;
        scrollView = parentScrollView;

        // UI ¼¼ÆÃ
        if (iconImage != null) iconImage.sprite = humanCardData.cardImage;
        if (nameText != null) nameText.text = humanCardData.cardName;

        UIBarUtility.SetBarColor(staminaBar, (int)humanCardData.Stamina, UIBarUtility.StaminaColor);
        UIBarUtility.SetBarColor(strengthBar, (int)humanCardData.AttackPower, UIBarUtility.StrengthColor);
        UIBarUtility.SetBarColor(mentalBar, (int)humanCardData.ConsumeHunger, UIBarUtility.WarningColor);

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() =>
        {
            exploreInfoUI.SetHumanInfo(_human);
            scrollView.SetActive(false);
        });
    }
}

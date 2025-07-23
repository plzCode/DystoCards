using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExploreInfo : MonoBehaviour
{
    [Header("장소 UI 요소")]
    [SerializeField] private Image exploreImage;
    [SerializeField] private TextMeshProUGUI exploreName;
    [SerializeField] private GameObject exploreStaminaBar;
    [SerializeField] private GameObject exploreStrengthBar;
    [SerializeField] private GameObject exploreDangerBar;
    [SerializeField] private TextMeshProUGUI successPercent;

    [Header("인물 UI 요소")]
    [SerializeField] private Image humanImage;
    [SerializeField] private TextMeshProUGUI humanName;
    [SerializeField] private GameObject humanStaminaBar;
    [SerializeField] private GameObject humanStrengthBar;
    [SerializeField] private GameObject humanMentality;

    [Header("비활성 색상")]
    [SerializeField] private Color inactiveColor = new Color(1, 1, 1, 0.2f);

    [Header("각 Bar 별 활성 색상")]
    [SerializeField] private Color strengthColor = Color.red;
    [SerializeField] private Color staminaColor = Color.green;
    [SerializeField] private Color warningColor = Color.yellow;

    [Header("현재 장소 정보")]
    [SerializeField] private LocationInfo current_locationInfo;
    //[SerializeField] private ExplorationData current_ExplorationData;

    [Header("현재 인물 정보")]
    [SerializeField] private HumanCardData current_humanInfo;

    


    public void SetExploreInfo(LocationInfo _location)
    {
        current_locationInfo = _location;
        //current_ExplorationData.location = _location;

        exploreImage.sprite = _location.locationImage;
        exploreName.text = _location.locationName.ToString();
        UIBarUtility.SetBarColor(exploreStrengthBar, _location.requiredStrength, UIBarUtility.StrengthColor);
        UIBarUtility.SetBarColor(exploreStaminaBar, _location.requiredStamina, UIBarUtility.StaminaColor);
        UIBarUtility.SetBarColor(exploreDangerBar, _location.dangerLevel, UIBarUtility.WarningColor);
        successPercent.text = "Succes Percent : " +(100 - _location.dangerLevel * 10).ToString();
    }

    public void SetHumanInfo(HumanCardData _human)
    {
        current_humanInfo = _human;
        //current_ExplorationData.human = _human;

        humanImage.sprite = _human.cardImage;
        humanName.text = _human.cardName;
        UIBarUtility.SetBarColor(humanStaminaBar, (int)_human.stamina, UIBarUtility.StaminaColor);
        UIBarUtility.SetBarColor(humanStrengthBar, (int)_human.attack_power, UIBarUtility.StrengthColor);
        UIBarUtility.SetBarColor(humanMentality, (int)_human.consume_hunger, UIBarUtility.WarningColor);
        
        

    }

    /*
    private void SetBarColor(GameObject barObject, int value, Color activeColor)
    {
        for (int i = 0; i < barObject.transform.childCount; i++)
        {
            Image image = barObject.transform.GetChild(i).GetComponent<Image>();
            image.color = (i < value) ? activeColor : inactiveColor;
        }
    }
    */

    public void OnClick_ExploreRegister()
    {
        if (current_locationInfo != null && current_humanInfo != null)
        {
            bool success = ExploreManager.Instance.AddExplore(current_humanInfo, current_locationInfo);
            if (!success)
            {
               // Debug.Log("이미 해당 인원이 해당 지역을 탐색 중입니다.");
            }
        }
    }

}

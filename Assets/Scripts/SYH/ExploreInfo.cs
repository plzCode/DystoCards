using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExploreInfo : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private Image ExploreImage;
    [SerializeField] private TextMeshProUGUI ExploreName;
    [SerializeField] private GameObject StaminaBar;
    [SerializeField] private GameObject StrengthBar;
    [SerializeField] private GameObject WarningBar;
    [SerializeField] private TextMeshProUGUI SuccessPercent;

    [Header("비활성 색상")]
    [SerializeField] private Color inactiveColor = new Color(1, 1, 1, 0.2f);

    [Header("각 Bar 별 활성 색상")]
    [SerializeField] private Color strengthColor = Color.red;
    [SerializeField] private Color staminaColor = Color.green;
    [SerializeField] private Color warningColor = Color.yellow;

    [Header("현재 탐색 정보")]
    [SerializeField] private LocationInfo current_locationInfo;

    


    public void SetExploreInfo(LocationInfo location)
    {
        current_locationInfo = location;
        ExploreImage.sprite = location.locationImage;
        ExploreName.text = location.locationName.ToString();
        SetBarColor(StrengthBar, location.requiredStrength, strengthColor);
        SetBarColor(StaminaBar, location.requiredStamina, staminaColor);
        SetBarColor(WarningBar, location.dangerLevel, warningColor);
        SuccessPercent.text = "Succes Percent : " +(100 - location.dangerLevel * 10).ToString();
    }

    private void SetBarColor(GameObject barObject, int value, Color activeColor)
    {
        for (int i = 0; i < barObject.transform.childCount; i++)
        {
            Image image = barObject.transform.GetChild(i).GetComponent<Image>();
            image.color = (i < value) ? activeColor : inactiveColor;
        }
    }

    public void OnClick_ExploreRegister()
    {
        if (current_locationInfo != null)
        {
            bool success = ExploreManager.Instance.AddExplore(current_locationInfo);
            if (!success)
            {
                Debug.Log("이미 탐색 중입니다.");
            }
            
        }
    }

}

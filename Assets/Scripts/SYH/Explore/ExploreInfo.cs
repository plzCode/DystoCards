using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExploreInfo : MonoBehaviour
{
    [Header("��� UI ���")]
    [SerializeField] private Image exploreImage;
    [SerializeField] private TextMeshProUGUI exploreName;
    [SerializeField] private GameObject exploreStaminaBar;
    [SerializeField] private GameObject exploreStrengthBar;
    [SerializeField] private GameObject exploreDangerBar;
    [SerializeField] private TextMeshProUGUI successPercent;

    [Header("�ι� UI ���")]
    [SerializeField] private Image humanImage;
    [SerializeField] private TextMeshProUGUI humanName;
    [SerializeField] private GameObject humanStaminaBar;
    [SerializeField] private GameObject humanStrengthBar;
    [SerializeField] private GameObject humanMentality;

    [Header("��Ȱ�� ����")]
    [SerializeField] private Color inactiveColor = new Color(1, 1, 1, 0.2f);

    [Header("�� Bar �� Ȱ�� ����")]
    [SerializeField] private Color strengthColor = Color.red;
    [SerializeField] private Color staminaColor = Color.green;
    [SerializeField] private Color warningColor = Color.yellow;

    [Header("���� ��� ����")]
    [SerializeField] private LocationInfo current_locationInfo;
    //[SerializeField] private ExplorationData current_ExplorationData;

    [Header("���� �ι� ����")]
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
               // Debug.Log("�̹� �ش� �ο��� �ش� ������ Ž�� ���Դϴ�.");
            }
        }
    }

}

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
        //successPercent.text = "Succes Percent : " +(100 - _location.dangerLevel * 10).ToString();
        ExpressSuccessPercent(current_locationInfo,current_humanInfo);
    }

    public void SetHumanInfo(HumanCardData _human)
    {
        current_humanInfo = _human;
        //current_ExplorationData.human = _human;

        humanImage.sprite = _human.cardImage;
        humanName.text = _human.cardName;
        UIBarUtility.SetBarColor(humanStaminaBar, (int)_human.Stamina, UIBarUtility.StaminaColor);
        UIBarUtility.SetBarColor(humanStrengthBar, (int)_human.AttackPower, UIBarUtility.StrengthColor);
        UIBarUtility.SetBarColor(humanMentality, (int)_human.ConsumeHunger, UIBarUtility.WarningColor);
        ExpressSuccessPercent(current_locationInfo,current_humanInfo);

    }




    
    private void ExpressSuccessPercent(LocationInfo location, HumanCardData human)
    {
        float successRate = ExploreManager.Instance.CaculateSuccessPercent(location, human);
        if (successRate == 0)
        {
            successPercent.text = "Success Percent : -";
            return;
        }
        if (successRate == 1)
        {
            successPercent.text = "No Stamina";
            successPercent.color = Color.gray;
            return;
        }
            

        string statusText;
        Color statusColor;

        if (successRate <= 33f)
        {
            statusText = "����";
            statusColor = Color.red;
        }
        else if (successRate < 70f)
        {
            statusText = "����";
            statusColor = Color.yellow;
        }
        else if (successRate < 90f)
        {
            statusText = "����";
            statusColor = Color.green;
        }
        else
        {
            statusText = "�ſ� ����";
            statusColor = new Color(0.2f, 1f, 0.2f); // �� ���� �ʷ�
        }

        successPercent.text = $"Ž�� ���� : {statusText}";
        successPercent.color = statusColor;
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

    private bool IsExplorePossible()
    {
        if (current_humanInfo == null || current_locationInfo == null)
            return false;

        return current_humanInfo.Stamina >= current_locationInfo.requiredStamina;
    }

    public void OnClick_ExploreRegister()
    {
        if (!IsExplorePossible())
        {
            Debug.Log("���¹̳��� �����Ͽ� Ž���� �� �����ϴ�.");
            return;
        }

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

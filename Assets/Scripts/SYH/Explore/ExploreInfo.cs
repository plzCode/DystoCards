using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExploreInfo : MonoBehaviour
{
    [Header("장소 UI 요소")]
    [SerializeField] private Sprite defaultLocoImg;
    [SerializeField] private Image exploreImage;
    [SerializeField] private TextMeshProUGUI exploreName;
    [SerializeField] private GameObject exploreStaminaBar;
    [SerializeField] private GameObject exploreStrengthBar;
    [SerializeField] private GameObject exploreDangerBar;
    [SerializeField] private TextMeshProUGUI successPercent;

    [Header("인물 UI 요소")]
    [SerializeField] private Sprite defaultHumanImg;
    [SerializeField] private Image humanImage;
    [SerializeField] private TextMeshProUGUI humanName;
    [SerializeField] private GameObject humanStaminaBar;
    [SerializeField] private GameObject humanStrengthBar;
    [SerializeField] private GameObject humanMentality;

    [Header("현재 장소 정보")]
    [SerializeField] private LocationInfo current_locationInfo;
    //[SerializeField] private ExplorationData current_ExplorationData;

    [Header("현재 인물 정보")]
    [SerializeField] private Human current_humanInfo;

    [Header("보상 정보")]
    [SerializeField] private GameObject rewardGrid;
    [SerializeField] private GameObject rewardPrefab;




    public void SetExploreInfo(LocationInfo _location)
    {
        current_locationInfo = _location;
        //current_ExplorationData.location = _location;

        exploreImage.sprite = _location.locationImage;
        exploreName.text = _location.locationName.ToString();
        UIBarUtility.SetBarColor(exploreStrengthBar, _location.requiredStrength, UIBarUtility.StrengthColor);
        UIBarUtility.SetBarColor(exploreStaminaBar, _location.requiredStamina, UIBarUtility.StaminaColor);
        UIBarUtility.SetBarColor(exploreDangerBar, _location.durationDays, UIBarUtility.WarningColor);
        //successPercent.text = "Succes Percent : " +(100 - _location.dangerLevel * 10).ToString();
        if (current_locationInfo != null && current_humanInfo != null)
        {
            ExpressSuccessPercent(current_locationInfo, current_humanInfo.humanData);
        }
        
    }

    public void SetHumanInfo(Human _human)
    {
        current_humanInfo = _human;
        HumanCardData _humandata = _human.humanData;
        //current_ExplorationData.human = _human;

        humanImage.sprite = _humandata.cardImage;
        humanName.text = _humandata.cardName;
        UIBarUtility.SetBarColor(humanStaminaBar, (int)_humandata.Stamina, UIBarUtility.StaminaColor);
        UIBarUtility.SetBarColor(humanStrengthBar, (int)_humandata.AttackPower, UIBarUtility.StrengthColor);
        UIBarUtility.SetBarColor(humanMentality, (int)_humandata.CurrentMentalHealth, UIBarUtility.WarningColor);
        if (current_locationInfo != null && current_humanInfo != null)
        {
            ExpressSuccessPercent(current_locationInfo, current_humanInfo.humanData);
        }

    }




    
    private void ExpressSuccessPercent(LocationInfo location, HumanCardData human)
    {
        

        // 보상 정보 초기화
        for (int i = rewardGrid.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(rewardGrid.transform.GetChild(i).gameObject);
        }

        RewardList();

        float successRate = ExploreManager.Instance.CaculateSuccessPercent(location, human);
        if (successRate == 0)
        {
            successPercent.text = "Success Percent : -";
            return;
        }
        if (successRate == 1)
        {
            successPercent.text = "스태미나 부족";
            successPercent.color = Color.gray;
            return;
        }
            

        string statusText;
        Color statusColor;

        if (successRate <= 33f)
        {
            statusText = "위험";
            statusColor = Color.red;
        }
        else if (successRate < 70f)
        {
            statusText = "보통";
            statusColor = Color.yellow;
        }
        else if (successRate < 90f)
        {
            statusText = "안전";
            statusColor = Color.green;
        }
        else
        {
            statusText = "매우 안전";
            statusColor = new Color(0.2f, 1f, 0.2f); // 더 밝은 초록
        }

        successPercent.text = $"탐험 상태 : {statusText}";
        successPercent.color = statusColor;


        



    }

    private void RewardList()
    {
        List<RewardInfo> rewardInfo =  current_locationInfo.rewards;
        for(int i = 0; i < rewardInfo.Count; i++)
        {
            GameObject reward = Instantiate(rewardPrefab,rewardGrid.transform);
            reward.GetComponent<Image>().sprite = rewardInfo[i].card.cardImage;
            reward.GetComponentInChildren<TextMeshProUGUI>().text = "x " + rewardInfo[i].quantity;
        }
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

        return current_humanInfo.humanData.Stamina >= current_locationInfo.requiredStamina;
    }

    public void OnClick_ExploreRegister()
    {
        if (!IsExplorePossible())
        {
            Debug.Log("스태미나가 부족하여 탐험할 수 없습니다.");
            return;
        }

        if (current_locationInfo != null && current_humanInfo != null)
        {
            bool success = ExploreManager.Instance.AddExplore(current_humanInfo, current_locationInfo);
            if (!success)
            {
               // Debug.Log("이미 해당 인원이 해당 지역을 탐색 중입니다.");
            }
            else
            {
                transform.parent.gameObject.SetActive(false);
            }
        }

        
    }

    private void OnEnable()
    {
        Clear();
    }

    public void Clear()
    {
        // 장소 정보 초기화
        exploreImage.sprite = defaultLocoImg;
        exploreName.text = "탐험지";
        UIBarUtility.SetBarColor(exploreStrengthBar, 0, UIBarUtility.StrengthColor);
        UIBarUtility.SetBarColor(exploreStaminaBar, 0, UIBarUtility.StaminaColor);
        UIBarUtility.SetBarColor(exploreDangerBar, 0, UIBarUtility.WarningColor);

        // 인물 정보 초기화
        humanImage.sprite = defaultHumanImg;
        humanName.text = "탐험가";
        UIBarUtility.SetBarColor(humanStaminaBar, 0, UIBarUtility.StaminaColor);
        UIBarUtility.SetBarColor(humanStrengthBar, 0, UIBarUtility.StrengthColor);
        UIBarUtility.SetBarColor(humanMentality, 0, UIBarUtility.WarningColor);

        // 성공률 텍스트 초기화
        successPercent.text = "-";
        successPercent.color = Color.white;

        // 현재 정보 null 처리
        current_locationInfo = null;
        current_humanInfo = null;

        // 보상 정보 초기화
        for (int i = rewardGrid.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(rewardGrid.transform.GetChild(i).gameObject);
        }
    }

}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExploringInfo : MonoBehaviour
{
    public ExplorationData Data { get; private set; }

    [Header("UI요소")]
    [SerializeField] private Image humanImage;
    [SerializeField] private TextMeshProUGUI humanStrengthText;
    [SerializeField] private Image locationImage;
    [SerializeField] private TextMeshProUGUI locationStrengthText;    
    [SerializeField] private GameObject remainDaysBar;
    [SerializeField] private GameObject locationDangerBar;



    public void SetData(ExplorationData data)
    {
        Data = data;
        // UI에 데이터 반영 (예: 이름, 위치, 남은 일수 표시 등)
        HumanCardData humanInfo = data.human;
        LocationInfo locationInfo = data.location;

        humanImage.sprite = humanInfo.cardImage;
        humanStrengthText.text = humanInfo.AttackPower.ToString();
        locationImage.sprite = locationInfo.locationImage;
        locationStrengthText.text = locationInfo.requiredStrength.ToString();
        UIBarUtility.SetBarColor(remainDaysBar, data.remainingDays, UIBarUtility.StrengthColor);
        UIBarUtility.SetBarColor(locationDangerBar, locationInfo.dangerLevel, UIBarUtility.WarningColor);

        TurnManager.Instance.RegisterPhaseAction(TurnPhase.ExploreAction, () => { });
        TurnManager.Instance.RegisterPhaseAction(TurnPhase.ExploreEnd, () => RefreshRemainDays());

    }

    public void RefreshRemainDays()
    {
        UIBarUtility.SetBarColor(remainDaysBar, Data.remainingDays, UIBarUtility.StrengthColor);
    }
}

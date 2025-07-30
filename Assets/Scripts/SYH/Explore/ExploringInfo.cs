using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExploringInfo : MonoBehaviour
{
    public ExplorationData Data { get; private set; }

    [Header("UI���")]
    [SerializeField] private Image humanImage;
    [SerializeField] private TextMeshProUGUI humanStrengthText;
    [SerializeField] private Image locationImage;
    [SerializeField] private TextMeshProUGUI locationStrengthText;    
    [SerializeField] private GameObject remainDaysBar;
    [SerializeField] private GameObject locationDangerBar;



    public void SetData(ExplorationData data)
    {
        Data = data;
        // UI�� ������ �ݿ� (��: �̸�, ��ġ, ���� �ϼ� ǥ�� ��)
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

using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class StatUIEntry : MonoBehaviour
{
    public TextMeshProUGUI statNameText;
    public Image statIcon;

    [Header("Bar Mode")]
    public StatBarUI barUI;

    [Header("Number Mode")]
    public GameObject numberGroup;
    public TextMeshProUGUI statValueText;

    public void SetStat(string statName, float value, float maxValue, StatVisualType type)
    {        
        var icon = UIManager.Instance.iconDatabase.GetIcon(statName);
        var text = UIManager.Instance.iconDatabase.GetName(statName);
        statIcon.sprite = icon;
        statNameText.text = text != null ? text.text : statName;

        if (type == StatVisualType.Bar)
        {
            barUI.gameObject.SetActive(true);
            numberGroup.SetActive(false);
            barUI.Init(maxValue);
            barUI.SetValue(value);
        }
        else
        {
            barUI.gameObject.SetActive(false);
            numberGroup.SetActive(true);
            statValueText.text = value.ToString();
        }
        
    }
}

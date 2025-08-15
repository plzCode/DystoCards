using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenLocationInfo : MonoBehaviour
{

    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public void Set(LocationInfo info)
    {
        iconImage.sprite = info.locationImage;
        descriptionText.text = "'" + info.locationName + "' Å½»ç °³¹æ";
        
    }
}

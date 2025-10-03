using UnityEngine;
using UnityEngine.UI;

public static class UIBarUtility
{
    private static UIThemeData _theme;

    public static void Init(UIThemeData theme)
    {
        _theme = theme;
    }

    public static void SetBarColor(GameObject barObject, int value, Color activeColor)
    {
        if (barObject == null || _theme == null) return;

        for (int i = 0; i < barObject.transform.childCount; i++)
        {
            Image image = barObject.transform.GetChild(i).GetComponent<Image>();
            if (image != null)
            {
                image.color = (i < value) ? activeColor : InactiveColor;
            }
        }
    }

    // 색상 접근용 getter
    public static Color StrengthColor => _theme?.strengthColor ?? Color.red;
    public static Color StaminaColor => _theme?.staminaColor ?? Color.green;
    public static Color WarningColor => _theme?.warningColor ?? Color.yellow;
    public static Color InactiveColor => _theme?.inactiveColor ?? new Color(1f, 1f, 1f, 0.2f); 
}
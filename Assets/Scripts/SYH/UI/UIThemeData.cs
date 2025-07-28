using UnityEngine;

[CreateAssetMenu(menuName = "UI/Theme Data")]
public class UIThemeData : ScriptableObject
{
    [Header("활성 색상")]
    public Color staminaColor;
    public Color strengthColor;    
    public Color warningColor;

    [Header("비활성 색상")]
    public Color inactiveColor;
}

using UnityEngine;

[CreateAssetMenu(menuName = "UI/Theme Data")]
public class UIThemeData : ScriptableObject
{
    [Header("Ȱ�� ����")]
    public Color staminaColor;
    public Color strengthColor;    
    public Color warningColor;

    [Header("��Ȱ�� ����")]
    public Color inactiveColor;
}

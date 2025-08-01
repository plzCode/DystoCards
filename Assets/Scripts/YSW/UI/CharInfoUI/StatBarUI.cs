using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum StatVisualType
{
    Bar,    // 아이콘 나열
    Number  // 숫자 표시
}

public class StatBarUI : MonoBehaviour
{
    public RectTransform container;
    public GameObject iconPrefab;

    public float maxIconWidth = 32f;
    public float minIconWidth = 10f;

    public Color filledColor = Color.green;
    public Color emptyColor = Color.gray;

    private List<Image> icons = new();
    private float maxValue = 0;
    private float currentValue = 0;

    public void Init(float maxValue)
    {
        this.maxValue = maxValue;
        this.currentValue = maxValue;

        foreach (Transform child in container)
            Destroy(child.gameObject);
        icons.Clear();

        for (int i = 0; i < Mathf.RoundToInt(maxValue); i++)
        {
            GameObject icon = Instantiate(iconPrefab, container);
            Image img = icon.GetComponent<Image>();
            icons.Add(img);
        }

        ResizeIcons();
        UpdateVisual();
    }

    public void SetValue(float value)
    {
        currentValue = Mathf.Clamp(value, 0, maxValue);
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        for (int i = 0; i < icons.Count; i++)
        {
            icons[i].color = i < currentValue ? filledColor : emptyColor;
        }
    }

    private void ResizeIcons()
    {
        float containerWidth = container.rect.width;
        float iconWidth = Mathf.Clamp(containerWidth / maxValue, minIconWidth, maxIconWidth);
        float iconHeight = 33f;//iconPrefab.GetComponent<RectTransform>().sizeDelta.y;

        foreach (var img in icons)
        {
            RectTransform rt = img.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(iconWidth, iconHeight);
        }
    }
}

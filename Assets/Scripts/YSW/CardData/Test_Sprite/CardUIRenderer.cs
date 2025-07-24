using JetBrains.Annotations;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUIRenderer : MonoBehaviour
{
    
    public Transform statAnchor;  // 카드 위에 붙일 위치
    public GameObject statVisualPrefab;

    public TextMeshPro cardNameText;
    public float spacing = 0.2f;

    public void RenderStats(Dictionary<string, float> stats)
    {
        foreach (Transform child in statAnchor)
            Destroy(child.gameObject);

        var normalStats = new List<KeyValuePair<string, float>>();
        float sizeValue = 0f;
        bool hasSize = false;

        foreach (var kv in stats)
        {
            if (kv.Key == "size")
            {
                hasSize = true;
                sizeValue = kv.Value;
            }
            else
            {
                normalStats.Add(kv);
            }
        }

        // ========== 설정 ==========
        int maxPerRow = 4;
        float spacingX = 0.35f;
        float spacingY = 0.25f;
        float startY = -0.3f;
        float iconTextGap = 0.075f;

        // 스탯 개수 + size 포함 여부
        int totalStatCount = normalStats.Count + (hasSize ? 1 : 0);

        int rowCount = totalStatCount <= maxPerRow ? 1 : 2;


        // Scale 조정 나머지 구해서 0이면 4로 취급
        int mod = totalStatCount % 4;
        if (mod == 0 && totalStatCount != 0) mod = 4;

        float scale = 1.9f - 0.1f * mod; // 1.4 - 0.1 * [1~4] → 1.5 ~ 1.1
        statAnchor.localScale = Vector3.one * scale;

        // === 일반 스탯 배치 ===
        for (int i = 0; i < normalStats.Count; i++)
        {
            int index = i;
            int row = (rowCount == 2 && index >= maxPerRow) ? 1 : 0;
            int col = (row == 0) ? index : index - maxPerRow;

            int countInRow = (row == 0)
                ? Mathf.Min(maxPerRow, totalStatCount) // 🔧 고침
                : totalStatCount - Mathf.Min(maxPerRow, totalStatCount); // 🔧 고침

            Vector3 pos = GetStatPosition(col, countInRow, row, startY, spacingX, spacingY);
            CreateStatVisual(normalStats[i].Key, normalStats[i].Value, pos, iconTextGap);
        }

        // === size 배치 ===
        if (hasSize)
        {
            int sizeRow = (rowCount == 1 || (normalStats.Count < maxPerRow)) ? 0 : 1;
            int sizeCol = (sizeRow == 0)
                ? normalStats.Count
                : totalStatCount - maxPerRow - 1;

            int countInRow = (sizeRow == 0)
                ? totalStatCount
                : totalStatCount - maxPerRow;

            Vector3 pos = GetStatPosition(sizeCol, countInRow, sizeRow, startY, spacingX, spacingY);
            CreateStatVisual("size", sizeValue, pos, iconTextGap);
        }

    }
    public void RenderName(string name)
    {
        if(cardNameText != null)
        {
            cardNameText.text = name;
        }
    }
    private Vector3 GetStatPosition(int index, int countInRow, int rowIndex, float startY, float spacingX, float spacingY)
    {
        float rowY = startY - rowIndex * spacingY;
        float totalWidth = spacingX * (countInRow - 1);
        float startX = -totalWidth / 2f;
        return new Vector3(startX + index * spacingX, rowY, 0f);
    }

    private void CreateStatVisual(string statName, float value, Vector3 localPosition, float iconTextGap)
    {
        GameObject statObj = Instantiate(statVisualPrefab, statAnchor);
        statObj.name = statName;
        statObj.transform.localPosition = localPosition;

        var icon = UIManager.Instance.iconDatabase.GetIcon(statName);
        var iconRenderer = statObj.transform.Find("Icon").GetComponent<SpriteRenderer>();
        var valueText = statObj.transform.Find("Value").GetComponent<TextMeshPro>();

        iconRenderer.sprite = icon;
        valueText.text = value.ToString("0.#");

        iconRenderer.transform.localPosition = new Vector3(-iconTextGap, 0f, 0f);
        valueText.transform.localPosition = new Vector3(iconTextGap, 0f, 0f);
    }

}
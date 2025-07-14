using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUIRenderer : MonoBehaviour
{
    public StatIconDatabase iconDatabase;
    public Transform statAnchor;  // ī�� ���� ���� ��ġ
    public GameObject statVisualPrefab;

    public float spacing = 0.4f;

    public void RenderStats(Dictionary<string, float> stats)
    {
        // ���� ������ ����
        foreach (Transform child in statAnchor)
            Destroy(child.gameObject);

        int index = 0;
        foreach (var kv in stats)
        {
            GameObject statObj = Instantiate(statVisualPrefab, statAnchor);
            statObj.transform.localPosition = new Vector3(index * spacing, 0f, 0f); // ī�� ������ ���η� ����

            var icon = iconDatabase.GetIcon(kv.Key);
            var iconRenderer = statObj.transform.Find("Icon").GetComponent<SpriteRenderer>();
            var valueText = statObj.transform.Find("Value").GetComponent<TextMeshPro>();

            iconRenderer.sprite = icon;
            valueText.text = kv.Value.ToString("0.#");

            index++;
        }
    }
}
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DaySummary : MonoBehaviour
{
    public static DaySummary Instance { get; private set; }
    void Awake() => Instance = this;

    [Header("UI")]
    [SerializeField] GameObject panel;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI list;
    [SerializeField] Button btnClose;

    // id -> count
    Dictionary<string, int> bank = new();

    void Start()
    {
        if (btnClose) btnClose.onClick.AddListener(Close);
        Close(); // 시작 시 닫아두기
    }

    // === 누적 ===
    public void Add(string id, int count)
    {
        if (string.IsNullOrEmpty(id) || count <= 0) return;
        if (!bank.ContainsKey(id)) bank[id] = 0;
        bank[id] += count;
    }

    // === 팝업 열기 ===
    public void Open(string dayLabel = null)
    {
        if (title) title.text = dayLabel ?? "Day Result";
        if (list) list.text = BuildText();
        if (panel) panel.SetActive(true);
    }

    // === 팝업 + 즉시 내부 초기화 ===
    // 팝업에 적힌 문자열(list.text)은 그대로 남으므로, 사용자가 닫기 전까지 내용은 보입니다.
    public void ShowPopupAndClear(string dayLabel = null)
    {
        Open(dayLabel);
        bank.Clear();              // 내부 누적은 다음 날을 위해 즉시 초기화
    }

    public static void ShowPopupAndClearStatic(string dayLabel = null)
    {
        Instance?.ShowPopupAndClear(dayLabel);
    }

    // === 닫기 ===
    public void Close()
    {
        if (panel) panel.SetActive(false);
    }

    // === 완전 초기화(팝업 텍스트까지 비움) ===
    public void Clear()
    {
        bank.Clear();
        if (list) list.text = "";
    }

    // === 문자열 생성 ===
    string BuildText()
    {
        if (bank.Count == 0) return "No rewards today.";
        var sb = new StringBuilder();
        foreach (var kv in bank)
            sb.AppendLine($"{kv.Key} x {kv.Value}");
        return sb.ToString();
    }
}
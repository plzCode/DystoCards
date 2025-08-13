using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ��ü�� ����� �����ϴ� Ŭ����
/// </summary>
public class Recorder : MonoBehaviour
{
    public static Recorder Instance { get; private set; }

    private readonly List<HumanRecordInfo> _humanRecords = new List<HumanRecordInfo>();
    public IReadOnlyList<HumanRecordInfo> HumanRecords => _humanRecords.AsReadOnly();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //  HumanCard �߰�
    public void AddHuman(string humanName, int day)
    {
        if (_humanRecords.Exists(h => h.HumanName == humanName))
            return; // �̹� ������ ����

        _humanRecords.Add(new HumanRecordInfo(humanName, day));
    }

    //  ���� ���
    public void RecordHumanFood(string humanName, string foodName, int amount = 1)
    {
        var human = _humanRecords.Find(h => h.HumanName == humanName);
        if (human != null)
        {
            human.UseFood(foodName, amount);
        }
    }

    //  ��� ���
    public void RecordHumanDeath(string humanName, string cause, int day)
    {
        var human = _humanRecords.Find(h => h.HumanName == humanName);
        if (human != null)
        {
            human.SetDeath(cause, day);
        }
    }
}


/// <summary>
/// ĳ���ͺ� ��� ����
/// </summary>
public class HumanRecordInfo
{
    public string HumanName { get; }
    public int joinDay { get; private set; } = -1;
    private readonly Dictionary<string, int> _eatFoods;
    public IReadOnlyDictionary<string, int> EatFoods => _eatFoods;
    public string DeathReason { get; private set; }
    public int DeathDay { get; private set; }

    public HumanRecordInfo(string humanName, int day)
    {
        HumanName = humanName;
        joinDay = day;
        _eatFoods = new Dictionary<string, int>();
        DeathReason = string.Empty;
        DeathDay = -1;
    }

    public HumanRecordInfo(string humanName)
    {
        HumanName = humanName;
        joinDay = -1; // �ʱⰪ -1�� ����
        _eatFoods = new Dictionary<string, int>();
        DeathReason = string.Empty;
        DeathDay = -1;

    }

    /// <summary>
    /// ���� ��� ���
    /// </summary>
    public void UseFood(string food, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(food) || amount <= 0)
            return;

        if (_eatFoods.ContainsKey(food))
            _eatFoods[food] += amount;
        else
            _eatFoods[food] = amount;
    }

    /// <summary>
    /// ��� ���� ���
    /// </summary>
    public void SetDeath(string reason, int day)
    {
        DeathReason = reason;
        DeathDay = day;
    }

    /// <summary>
    /// ���� ���� ���� ���� ��ȯ (������ null)
    /// </summary>
    public string GetFavoriteFood()
    {
        if (_eatFoods.Count == 0) return null;
        int maxCount = 0;
        string favorite = null;
        foreach (var kvp in _eatFoods)
        {
            if (kvp.Value > maxCount)
            {
                maxCount = kvp.Value;
                favorite = kvp.Key;
            }
        }
        return favorite;
    }
}

/// <summary>
/// �̺�Ʈ �α�
/// </summary>
public class EventLog
{
    public string EventName { get; }
    public int EventDay { get; }

    public EventLog(string eventName, int eventDay)
    {
        EventName = eventName;
        EventDay = eventDay;
    }
}

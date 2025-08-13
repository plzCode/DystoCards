using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 전체의 기록을 관리하는 클래스
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

    //  HumanCard 추가
    public void AddHuman(string humanName, int day)
    {
        if (_humanRecords.Exists(h => h.HumanName == humanName))
            return; // 이미 있으면 무시

        _humanRecords.Add(new HumanRecordInfo(humanName, day));
    }

    //  음식 기록
    public void RecordHumanFood(string humanName, string foodName, int amount = 1)
    {
        var human = _humanRecords.Find(h => h.HumanName == humanName);
        if (human != null)
        {
            human.UseFood(foodName, amount);
        }
    }

    //  사망 기록
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
/// 캐릭터별 기록 정보
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
        joinDay = -1; // 초기값 -1로 설정
        _eatFoods = new Dictionary<string, int>();
        DeathReason = string.Empty;
        DeathDay = -1;

    }

    /// <summary>
    /// 음식 사용 기록
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
    /// 사망 정보 기록
    /// </summary>
    public void SetDeath(string reason, int day)
    {
        DeathReason = reason;
        DeathDay = day;
    }

    /// <summary>
    /// 가장 많이 먹은 음식 반환 (없으면 null)
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
/// 이벤트 로그
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

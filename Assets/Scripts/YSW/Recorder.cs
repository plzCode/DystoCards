using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// ���� ��ü�� ����� �����ϴ� Ŭ����
/// </summary>
public class Recorder : MonoBehaviour
{
    public static Recorder Instance { get; private set; }

    private readonly List<HumanRecordInfo> _humanRecords = new List<HumanRecordInfo>();
    public IReadOnlyList<HumanRecordInfo> HumanRecords => _humanRecords.AsReadOnly();

    public List<EventLog> eventLogs = new List<EventLog>();

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
    public void RecordEvent(string contents, int Day, string eventFunctionKey)
    {
        eventLogs.Add(new EventLog(contents, Day, eventFunctionKey));
    }

    // ���
    public List<(int day, string message, int priority)> GetHumanStory()
    {
        List<(int, string, int)> story = new List<(int, string, int)>();
        foreach (var human in _humanRecords)
        {
            if (human.joinDay >= 1)
            {
                story.Add((human.joinDay, $"{human.HumanName}�� �������ϴ�.", 2));
            }
        }
        return story; 
    }

    public List<(int day, string messaeg, int priority)> GetHumanDeath()
    {
        List<(int, string, int)> story = new List<(int, string, int)>();
        foreach (var human in _humanRecords) 
        {            
            if (human.DeathDay >= 0)
            {
                string favoriteFood = human.GetFavoriteFood(); // ���� ���� ���� �߿��� ����������
                string deathMessage = $"{human.HumanName}�� ����߽��ϴ�. ����: {human.DeathReason}";
                if (!string.IsNullOrEmpty(favoriteFood))
                {
                    deathMessage += $"\n{favoriteFood}�� �� �����߾��µ�.";
                }

                story.Add((human.DeathDay, deathMessage, 9));
            }
        }
        return story;
    }

    public List<(int day, string message, int priority)> GetEventStroy()
    {
        List<(int, string, int)> story = new List<(int, string, int)>();
        int order = -1;
        foreach (var log in eventLogs)
        {
            switch (log.EventFunctionKey)
            {
                case "SpawnEnemy":
                    order = 3;
                    break;
                case "RecruitHuman":
                    order = 1;
                    break;
                default:
                    order = 3;
                    break;

            }


            story.Add((log.EventDay, log.EventName, order));
        }
        return story;
    }

    public List<(int day, string message, int priority)> GetAllStory()
    {
        // �� ���丮 ��ġ��
        List<(int day, string message, int priority) > story = new List<(int, string, int)>();
        story.AddRange(GetHumanStory());        
        story.AddRange(GetEventStroy()); 
        story.AddRange(GetHumanDeath());

        story.Sort((a, b) =>
        {
            int dayComparison = a.day.CompareTo(b.day);
            if (dayComparison != 0)
                return dayComparison;

            return a.priority.CompareTo(b.priority);
        });

        return story;
    }


    /// <summary>
    /// ĳ���ͺ� ��� ����
    /// </summary>
    public class HumanRecordInfo
    {
        public string HumanName { get; }
        public int joinDay { get; private set; } = -1; //���� �� �� int / ������. << sting
        private readonly Dictionary<string, int> _eatFoods; //string
        public IReadOnlyDictionary<string, int> EatFoods => _eatFoods;
        public string DeathReason { get; private set; } // Daeth Day int / Death Reason << string 
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
        public string EventFunctionKey { get; }


        public EventLog(string eventName, int eventDay, string eventFunctionKey)
        {
            EventName = eventName;
            EventDay = eventDay;
            EventFunctionKey = eventFunctionKey;
        }
    }
}

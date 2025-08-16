using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 게임 전체의 기록을 관리하는 클래스
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
    public void RecordEvent(string contents, int Day, string eventFunctionKey)
    {
        eventLogs.Add(new EventLog(contents, Day, eventFunctionKey));
    }

    // 결과
    public List<(int day, string message, int priority)> GetHumanStory()
    {
        List<(int, string, int)> story = new List<(int, string, int)>();
        foreach (var human in _humanRecords)
        {
            if (human.joinDay >= 1)
            {
                story.Add((human.joinDay, $"{human.HumanName}을 만났습니다.", 2));
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
                string favoriteFood = human.GetFavoriteFood(); // 많이 먹은 음식 중에서 랜덤돌리기
                string deathMessage = $"{human.HumanName}이 사망했습니다. 원인: {human.DeathReason}";
                if (!string.IsNullOrEmpty(favoriteFood))
                {
                    deathMessage += $"\n{favoriteFood}를 참 좋아했었는데.";
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
        // 두 스토리 합치기
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
    /// 캐릭터별 기록 정보
    /// </summary>
    public class HumanRecordInfo
    {
        public string HumanName { get; }
        public int joinDay { get; private set; } = -1; //만난 일 수 int / 만났다. << sting
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
        public string EventFunctionKey { get; }


        public EventLog(string eventName, int eventDay, string eventFunctionKey)
        {
            EventName = eventName;
            EventDay = eventDay;
            EventFunctionKey = eventFunctionKey;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class ExploreManager : MonoBehaviour
{

    public List<HumanCardData> registedHumans = new List<HumanCardData>();
    public static ExploreManager Instance { get; private set; }

    // 여러 탐험지를 저장
    public List<ExplorationData> registeredExplorations = new List<ExplorationData>();

    [SerializeField] private UIThemeData uiThemeData; // UI 색상 정보

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        UIBarUtility.Init(uiThemeData);
    }

    public bool AddExplore(HumanCardData human, LocationInfo location)
    {
        foreach (var exploration in registeredExplorations)
        {
            if (exploration.human == human && exploration.location == location)
            {
                Debug.Log($"[중복 탐사] {human.cardName}은 이미 {location.locationName} 탐사 중입니다.");
                return false;
            }

            if (exploration.human == human)
            {
                Debug.Log($"[중복 탐사] {human.cardName}은 이미 다른 장소를 탐사 중입니다.");
                return false;
            }

            if (exploration.location == location)
            {
                Debug.Log($"[중복 탐사] {location.locationName}은 이미 다른 인물이 탐사 중입니다.");
                return false;
            }
        }

        registeredExplorations.Add(new ExplorationData(human,location));
        Debug.Log($"[ExploreManager] 탐색 등록됨: {human.cardName} → {location.locationName}");
        return true;
    }

    // 선택적으로: 전체 초기화 메서드
    public void ClearExplores()
    {
        registeredExplorations.Clear();
        Debug.Log("[ExploreManager] 모든 탐색 정보가 초기화됨.");
    }
}

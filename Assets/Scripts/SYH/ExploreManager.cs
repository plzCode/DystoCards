using System.Collections.Generic;
using UnityEngine;

public class ExploreManager : MonoBehaviour
{


    public static ExploreManager Instance { get; private set; }

    // 여러 탐험지를 저장
    public List<LocationInfo> registeredLocations = new List<LocationInfo>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool AddExplore(LocationInfo locationInfo)
    {
        if (registeredLocations.Contains(locationInfo))
        {
            return false; // 이미 등록됨
        }

        registeredLocations.Add(locationInfo);
        Debug.Log($"[ExploreManager] 탐색 등록됨: {locationInfo.locationName}");
        return true;
    }

    // 선택적으로: 전체 초기화 메서드
    public void ClearExplores()
    {
        registeredLocations.Clear();
        Debug.Log("[ExploreManager] 모든 탐색 정보가 초기화됨.");
    }
}

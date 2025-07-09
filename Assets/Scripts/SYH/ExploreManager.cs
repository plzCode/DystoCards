using System.Collections.Generic;
using UnityEngine;

public class ExploreManager : MonoBehaviour
{


    public static ExploreManager Instance { get; private set; }

    // ���� Ž������ ����
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
            return false; // �̹� ��ϵ�
        }

        registeredLocations.Add(locationInfo);
        Debug.Log($"[ExploreManager] Ž�� ��ϵ�: {locationInfo.locationName}");
        return true;
    }

    // ����������: ��ü �ʱ�ȭ �޼���
    public void ClearExplores()
    {
        registeredLocations.Clear();
        Debug.Log("[ExploreManager] ��� Ž�� ������ �ʱ�ȭ��.");
    }
}

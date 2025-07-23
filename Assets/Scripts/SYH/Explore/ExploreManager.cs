using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class ExploreManager : MonoBehaviour
{

    public List<HumanCardData> registedHumans = new List<HumanCardData>();
    public static ExploreManager Instance { get; private set; }

    // ���� Ž������ ����
    public List<ExplorationData> registeredExplorations = new List<ExplorationData>();

    [SerializeField] private UIThemeData uiThemeData; // UI ���� ����

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
                Debug.Log($"[�ߺ� Ž��] {human.cardName}�� �̹� {location.locationName} Ž�� ���Դϴ�.");
                return false;
            }

            if (exploration.human == human)
            {
                Debug.Log($"[�ߺ� Ž��] {human.cardName}�� �̹� �ٸ� ��Ҹ� Ž�� ���Դϴ�.");
                return false;
            }

            if (exploration.location == location)
            {
                Debug.Log($"[�ߺ� Ž��] {location.locationName}�� �̹� �ٸ� �ι��� Ž�� ���Դϴ�.");
                return false;
            }
        }

        registeredExplorations.Add(new ExplorationData(human,location));
        Debug.Log($"[ExploreManager] Ž�� ��ϵ�: {human.cardName} �� {location.locationName}");
        return true;
    }

    // ����������: ��ü �ʱ�ȭ �޼���
    public void ClearExplores()
    {
        registeredExplorations.Clear();
        Debug.Log("[ExploreManager] ��� Ž�� ������ �ʱ�ȭ��.");
    }
}

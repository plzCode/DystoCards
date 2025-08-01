using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoxManager : MonoBehaviour
{
    [SerializeField] private GameObject boxUIPanel;
    [SerializeField] private Button closeButton;
    [SerializeField] private Transform contentParent; // ScrollView Content (카드 UI가 붙는 부모)
    [SerializeField] private GameObject cardUIPrefab; // 카드 UI Prefab (CardUI)
    [SerializeField] private TMP_Text capacityText;

    public static BoxManager Instance { get; private set; } // 싱글톤 인스턴스
    public Transform ContentParent => contentParent;
    public GameObject CardUIPrefab => cardUIPrefab;
    public TMP_Text CapacityText => capacityText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬이 넘어가도 유지
    }

    private void Start()
    {
        boxUIPanel.SetActive(false);
        // 닫기 버튼에 리스너 등록
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseUI);
    }

    public void CloseUI()
    {
        UIManager.Instance.TogglePanel(boxUIPanel);
    }

    public void OpenUI()
    {
        UIManager.Instance.TogglePanel(boxUIPanel);
    }
}

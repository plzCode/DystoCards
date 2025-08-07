using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StorageManager : MonoBehaviour
{
    [SerializeField] private GameObject boxUIPanel;
    [SerializeField] private Button closeButton;
    [SerializeField] private Transform contentParent; // ScrollView Content (카드 UI가 붙는 부모)
    [SerializeField] private GameObject cardUIPrefab; // 카드 UI Prefab (CardUI)
    [SerializeField] private TMP_Text capacityText;

    public static StorageManager Instance { get; private set; } // 싱글톤 인스턴스
    public Transform ContentParent => contentParent;
    public GameObject CardUIPrefab => cardUIPrefab;
    public TMP_Text CapacityText => capacityText;
    public Card_Storage currentBox { get; private set; }

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

    private void ClearCardUI()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void CloseUI()
    {
        UIManager.Instance.TogglePanel(boxUIPanel);
        currentBox = null;
        ClearCardUI();
    }

    public void OpenUI(Card_Storage box)
    {
        currentBox = box;

        // 먼저 이전 데이터 초기화
        ClearCardUI();

        // 새 박스의 내용으로 갱신
        box.UpdateBoxData();
        box.UpdateCardUI();

        UIManager.Instance.TogglePanel(boxUIPanel);
    }
}

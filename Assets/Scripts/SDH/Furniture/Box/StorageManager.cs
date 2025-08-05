using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StorageManager : MonoBehaviour
{
    [SerializeField] private GameObject boxUIPanel;
    [SerializeField] private Button closeButton;
    [SerializeField] private Transform contentParent; // ScrollView Content (ī�� UI�� �ٴ� �θ�)
    [SerializeField] private GameObject cardUIPrefab; // ī�� UI Prefab (CardUI)
    [SerializeField] private TMP_Text capacityText;

    public static StorageManager Instance { get; private set; } // �̱��� �ν��Ͻ�
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
        DontDestroyOnLoad(gameObject); // ���� �Ѿ�� ����
    }

    private void Start()
    {
        boxUIPanel.SetActive(false);
        // �ݱ� ��ư�� ������ ���
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

        // ���� ���� ������ �ʱ�ȭ
        ClearCardUI();

        // �� �ڽ��� �������� ����
        box.UpdateBoxData();
        box.UpdateCardUI();

        UIManager.Instance.TogglePanel(boxUIPanel);
    }
}

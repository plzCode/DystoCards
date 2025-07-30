using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoxManager : MonoBehaviour
{
    [SerializeField] private GameObject boxUICanvas;
    [SerializeField] private Button closeButton;
    [SerializeField] private Transform contentParent; // ScrollView Content (ī�� UI�� �ٴ� �θ�)
    [SerializeField] private GameObject cardUIPrefab; // ī�� UI Prefab (CardUI)
    [SerializeField] private TMP_Text capacityText;

    public static BoxManager Instance { get; private set; } // �̱��� �ν��Ͻ�
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
        DontDestroyOnLoad(gameObject); // ���� �Ѿ�� ����
    }

    private void Start()
    {
        // �ݱ� ��ư�� ������ ���
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseUI);
    }

    public void CloseUI()
    {
        boxUICanvas.SetActive(false);
    }

    public void OpenUI()
    {
        boxUICanvas.SetActive(true);
    }
}

using UnityEngine;
using UnityEngine.UI;

public class BoxManager : MonoBehaviour
{
    [SerializeField] private GameObject boxUICanvas;
    [SerializeField] private Button closeButton;
    [SerializeField] private Transform contentParent; // ScrollView Content (ī�� UI�� �ٴ� �θ�)
    [SerializeField] private GameObject cardUIPrefab; // ī�� UI Prefab (CardUI)

    public static BoxManager Instance { get; private set; } // �̱��� �ν��Ͻ�
    public Transform ContentParent => contentParent;
    public GameObject CardUIPrefab => cardUIPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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

using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ����� UI�� ī�� ������ ����ϴ� �Ŵ��� Ŭ����
/// �̱������� �����Ǿ� ������, ī�� UI ����, �ʱ�ȭ, ����/�ݱ� ����� ����
/// </summary>
public class StorageManager : MonoBehaviour
{
    [SerializeField] private GameObject boxUIPanel;   // ����� UI �г�
    [SerializeField] private Button closeButton;      // ����� UI �ݱ� ��ư
    [SerializeField] private Transform contentParent; // ScrollView Content (ī�� UI�� �ٴ� �θ�)
    [SerializeField] private GameObject cardUIPrefab; // ī�� UI Prefab (CardUI)
    [SerializeField] private TMP_Text capacityText;   // ����� �뷮 �ؽ�Ʈ ǥ��

    public static StorageManager Instance { get; private set; } // �̱��� �ν��Ͻ�
    public Transform ContentParent => contentParent;            // �ܺο��� contentParent ���ٿ�
    public GameObject CardUIPrefab => cardUIPrefab;             // ī�� UI ������ ���ٿ�
    public TMP_Text CapacityText => capacityText;               // �뷮 �ؽ�Ʈ ���ٿ�
    public Card_Storage currentBox { get; private set; }        // ���� �����ִ� ����� �ڽ�

    private void Awake()
    {
        // �̱��� �ν��Ͻ� �ʱ�ȭ
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // �� ��ȯ �� ������Ʈ ����
    }

    private void Start()
    {
        boxUIPanel.SetActive(false); // ���� �� UI ��Ȱ��ȭ

        // �ݱ� ��ư�� CloseUI �޼��� ������ ���
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseUI);
    }

    // ī�� UI�� ��� �����Ͽ� �ʱ�ȭ
    private void ClearCardUI()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// ����� UI�� �ݰ� ���� �ڽ� ���� �� ī�� UI �ʱ�ȭ
    /// </summary>
    public void CloseUI()
    {
        UIManager.Instance.TogglePanel(boxUIPanel); // UI ���
        currentBox = null; // ���� �ڽ� �ʱ�ȭ
        ClearCardUI(); // ī�� UI �ʱ�ȭ
    }

    /// <summary>
    /// ����� UI�� ���� ������ �ڽ��� �����͸� �ҷ��� ����
    /// </summary>
    /// <param name="box">�� ����� �ڽ�</param>
    public void OpenUI(Card_Storage box)
    {
        currentBox = box;

        ClearCardUI(); // ���� UI �ʱ�ȭ

        box.UpdateBoxData(); // �ڽ� ������ ����
        box.UpdateCardUI(); // ī�� UI ����

        UIManager.Instance.TogglePanel(boxUIPanel); // UI ���(���̱�)
    }
}

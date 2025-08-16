using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ����� UI�� ī�� ������ ����ϴ� �Ŵ��� Ŭ����
/// �̱������� �����Ǿ� ������, ī�� UI ����, �ʱ�ȭ, ����/�ݱ� ����� ����
/// </summary>
public class StorageManager : MonoBehaviour
{
    [SerializeField] private GameObject boxUIPanel;      // ����� UI �г�
    [SerializeField] private Button closeButton;         // ����� UI �ݱ� ��ư
    [SerializeField] private Transform contentParent;    // ScrollView Content (ī�� UI�� �ٴ� �θ�)
    [SerializeField] private GameObject cardUIPrefab;    // ī�� UI Prefab (CardUI)
    [SerializeField] private TMP_Text capacityText;      // ����� �뷮 �ؽ�Ʈ ǥ��
    [SerializeField] private Image backgroundImage;      // UI ��� �̹���
    [SerializeField] private Sprite[] backgroundSprites; // ī�� ID�� ���� ������ ��������Ʈ �迭
    [SerializeField] private GameObject fieldCards;      // FieldCards

    public static StorageManager Instance { get; private set; } // �̱��� �ν��Ͻ�
    public Transform ContentParent => contentParent;            // �ܺο��� contentParent ���ٿ�
    public GameObject CardUIPrefab => cardUIPrefab;             // ī�� UI ������ ���ٿ�
    public TMP_Text CapacityText => capacityText;               // �뷮 �ؽ�Ʈ ���ٿ�
    public GameObject FieldCards => fieldCards;                 // �ʵ� ī�� ���ٿ�
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
        AudioManager.Instance.PlaySFX("StorageClose");
    }

    /// <summary>
    /// ����� UI�� ���� ������ �ڽ��� �����͸� �ҷ��� ����
    /// </summary>
    /// <param name="box">�� ����� �ڽ�</param>
    public void OpenUI(Card_Storage box)
    {
        currentBox = box;

        string cardId = currentBox.card.cardData.cardId;

        if (cardId == "055") // ���� ����
            backgroundImage.sprite = backgroundSprites[0];
        else if (cardId == "056") // ö ����
            backgroundImage.sprite = backgroundSprites[1];
        else if (cardId == "052") // �����
            backgroundImage.sprite = backgroundSprites[2];

        ClearCardUI(); // ���� UI �ʱ�ȭ

        box.UpdateBoxData(); // �ڽ� ������ ����
        box.UpdateCardUI(); // ī�� UI ����

        UIManager.Instance.TogglePanel(boxUIPanel); // UI ���(���̱�)
        Debug.Log("!!!");
        AudioManager.Instance.PlaySFX("StorageOpen");
    }
}

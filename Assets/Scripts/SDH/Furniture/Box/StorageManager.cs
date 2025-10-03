using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 저장소 UI와 카드 관리를 담당하는 매니저 클래스
/// 싱글톤으로 구현되어 있으며, 카드 UI 생성, 초기화, 열기/닫기 기능을 제공
/// </summary>
public class StorageManager : MonoBehaviour
{
    [SerializeField] private GameObject boxUIPanel;      // 저장소 UI 패널
    [SerializeField] private Button closeButton;         // 저장소 UI 닫기 버튼
    [SerializeField] private Transform contentParent;    // ScrollView Content (카드 UI가 붙는 부모)
    [SerializeField] private GameObject cardUIPrefab;    // 카드 UI Prefab (CardUI)
    [SerializeField] private TMP_Text capacityText;      // 저장소 용량 텍스트 표시
    [SerializeField] private Image backgroundImage;      // UI 배경 이미지
    [SerializeField] private Sprite[] backgroundSprites; // 카드 ID에 따라 변경할 스프라이트 배열
    [SerializeField] private GameObject fieldCards;      // FieldCards

    public static StorageManager Instance { get; private set; } // 싱글톤 인스턴스
    public Transform ContentParent => contentParent;            // 외부에서 contentParent 접근용
    public GameObject CardUIPrefab => cardUIPrefab;             // 카드 UI 프리팹 접근용
    public TMP_Text CapacityText => capacityText;               // 용량 텍스트 접근용
    public GameObject FieldCards => fieldCards;                 // 필드 카드 접근용
    public Card_Storage currentBox { get; private set; }        // 현재 열려있는 저장소 박스

    private void Awake()
    {
        // 싱글톤 인스턴스 초기화
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시 오브젝트 유지
    }

    private void Start()
    {
        boxUIPanel.SetActive(false); // 시작 시 UI 비활성화

        // 닫기 버튼에 CloseUI 메서드 리스너 등록
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseUI);
    }

    // 카드 UI를 모두 삭제하여 초기화
    private void ClearCardUI()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// 저장소 UI를 닫고 현재 박스 참조 및 카드 UI 초기화
    /// </summary>
    public void CloseUI()
    {
        UIManager.Instance.TogglePanel(boxUIPanel); // UI 토글
        currentBox = null; // 현재 박스 초기화
        ClearCardUI(); // 카드 UI 초기화
        AudioManager.Instance.PlaySFX("StorageClose");
    }

    /// <summary>
    /// 저장소 UI를 열고 지정된 박스의 데이터를 불러와 갱신
    /// </summary>
    /// <param name="box">열 저장소 박스</param>
    public void OpenUI(Card_Storage box)
    {
        currentBox = box;

        string cardId = currentBox.card.cardData.cardId;

        if (cardId == "055") // 나무 상자
            backgroundImage.sprite = backgroundSprites[0];
        else if (cardId == "056") // 철 상자
            backgroundImage.sprite = backgroundSprites[1];
        else if (cardId == "052") // 냉장고
            backgroundImage.sprite = backgroundSprites[2];

        ClearCardUI(); // 이전 UI 초기화

        box.UpdateBoxData(); // 박스 데이터 갱신
        box.UpdateCardUI(); // 카드 UI 갱신

        UIManager.Instance.TogglePanel(boxUIPanel); // UI 토글(보이기)
        Debug.Log("!!!");
        AudioManager.Instance.PlaySFX("StorageOpen");
    }
}

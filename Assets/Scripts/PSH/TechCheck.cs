using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 기술 카드 버튼을 제어하는 컴포넌트
/// - 연결된 TechCardData가 해금되었는지 확인
/// - 해금되지 않은 경우 버튼을 통해 해금 처리
/// - 해금 시 해당 레시피도 RecipeBook에 등록됨
/// </summary>
public class TechCheck : MonoBehaviour
{

    [Header("기술 데이터 (ScriptableObject)")]
    [SerializeField] private TechCardData techCard; // 이 버튼이 제어할 기술

    [Header("연결된 오브젝트")]
    [SerializeField] private GameObject techObject;         // 현재 기술 카드 오브젝트 (파괴 대상)
    [SerializeField] private GameObject techResultPrefab;   // 해금 후 생성될 프리팹
    [SerializeField] private GameObject techTooltipUI;      // 툴팁 UI
    [SerializeField] private GameObject uiToClose;          // 버튼 누를 시 닫을 UI

    private Button button;
    private Image buttonImage;
    private Text buttonText;

    private bool alreadyDisabled = false;

    private void Start()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<Text>();

        if (techCard == null)
        {
            Debug.LogError("[TechCheck] TechCardData가 연결되지 않았습니다.");
            return;
        }

        //// 이미 연구 완료된 경우 → 버튼 비활성화
        //if (TechBook.Instance.IsUnlocked(techCard))
        //{
        //    Debug.Log($"기술 이미 해금됨: {techCard.cardName}");
        //    DisableButtonCompletely();
        //}
        //else
        //{
        //    Debug.Log($" 기술 잠금 상태: {techCard.cardName}");
        //}
    }

    /// <summary>
    /// 버튼 클릭 시 실행되는 함수 (UI에서 연결 필요)
    /// </summary>
    public void OnTechButtonClicked()
    {
        if (techCard == null)
        {
            Debug.LogWarning("[TechCheck] techCard가 설정되지 않았습니다.");
            return;
        }

        // 기술 연구 해금 
        //Instance.UnlockTech(techCard); // 내부에서 RecipeBook 처리 포함

        // UI 처리
        if (uiToClose != null) uiToClose.SetActive(false);
        if (techTooltipUI != null) techTooltipUI.SetActive(false);
        if (techObject != null) Destroy(techObject);

        // 해금된 기술 결과 프리팹 생성
        if (techResultPrefab != null)
            Instantiate(techResultPrefab, techObject.transform.position, Quaternion.identity);

        //if (uiToClose != null)
        //{
        //    uiToClose.SetActive(false); // UI 창 끄기
        //    Instantiate(SelectTech, Tech.transform.position, Quaternion.identity); // 선택한 테크카드 생성
        //    Destroy(Tech);
        //    TechTip.SetActive(false);
        //}




        //DisableButtonCompletely();
    }

    /// <summary>
    /// 버튼을 완전히 비활성화 (중복 눌림 방지, UI 효과 제거)
    /// </summary>
    private void DisableButtonCompletely()
    {
        if (alreadyDisabled) return;
        alreadyDisabled = true;

        if (button != null) button.interactable = false;
        if (buttonImage != null) buttonImage.raycastTarget = false;
        if (buttonText != null) buttonText.raycastTarget = false;
    }
}

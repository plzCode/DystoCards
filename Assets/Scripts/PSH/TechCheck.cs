using JetBrains.Annotations;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TechCheck : MonoBehaviour
{
    [SerializeField]
    public string itemName;

    public bool testbool = false;

    [SerializeField] private GameObject uiToClose; // 끌 UI 오브젝트

    private Button button;
    private Image buttonImage;
    private Text buttonText;

    private bool alreadyDisabled = false;

    void Start()
    {
        button = GetComponent<Button>(); // 자기 자신에 Button이 있는 경우
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<Text>(); // 혹은 TMP_Text
        string trimmedName = itemName.Trim();

        if (TechLoader.Instance == null)
        {
            Debug.LogError("TechLoader 인스턴스를 찾을 수 없습니다.");
            return;
        }

        if (TechLoader.Instance.researchedItems.Contains(trimmedName))
        {
            Debug.Log($"{trimmedName} 은(는) 연구 완료됨");

            DisableButtonCompletely();

        }
        else
        {
            Debug.Log($"{trimmedName} 은(는) 아직 연구되지 않음");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) // R 키를 누르면 출력
        {
            foreach (string item in TechLoader.Instance.researchedItems)
            {
                Debug.Log($"[연구됨] {item}");
            }
        }
    }


    public void test()
    {


        string trimmedName = itemName.Trim();

        if (!TechLoader.Instance.researchedItems.Contains(trimmedName))
        {
            TechLoader.Instance.researchedItems.Add(trimmedName);
            Debug.Log($"{trimmedName} 연구 완료");
        }

        DisableButtonCompletely();

        if (uiToClose != null)
            uiToClose.SetActive(false); // UI 창 끄기
    }


    private void DisableButtonCompletely()
    {
        if (alreadyDisabled) return;
        alreadyDisabled = true;

        // 버튼 비활성화
        if (button != null)
            button.interactable = false;

        // 눌림 효과 완전 제거
        if (buttonImage != null)
            buttonImage.raycastTarget = false;

        if (buttonText != null)
            buttonText.raycastTarget = false; // TMP_Text라면 TMP_Text로 교체
    }
}
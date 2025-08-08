using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfirmPopup : MonoBehaviour
{
    public static ConfirmPopup Instance;

    [SerializeField] private GameObject root;           // 패널 루트
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private Button yesBtn;
    [SerializeField] private Button noBtn;

    private System.Action onYes;

    private void Awake()
    {
        Instance = this;
        root.SetActive(false);

        yesBtn.onClick.AddListener(() =>
        {
            root.SetActive(false);
            onYes?.Invoke();
            UIManager.Instance.TogglePanel(root);       // 레이어 복귀
        });

        noBtn.onClick.AddListener(() =>
        {
            root.SetActive(false);
            UIManager.Instance.TogglePanel(root);
        });
    }

    public void Open(string msg, System.Action yesAction)
    {
        message.text = msg;
        onYes = yesAction;
        UIManager.Instance.TogglePanel(root);           // 패널 열기 + 입력 잠금
    }
}
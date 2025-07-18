using JetBrains.Annotations;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TechCheck : MonoBehaviour
{
    [SerializeField]
    public string itemName;

    public bool testbool = false;

    [SerializeField] private GameObject uiToClose; // �� UI ������Ʈ

    private Button button;
    private Image buttonImage;
    private Text buttonText;

    private bool alreadyDisabled = false;

    void Start()
    {
        button = GetComponent<Button>(); // �ڱ� �ڽſ� Button�� �ִ� ���
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<Text>(); // Ȥ�� TMP_Text
        string trimmedName = itemName.Trim();

        if (TechLoader.Instance == null)
        {
            Debug.LogError("TechLoader �ν��Ͻ��� ã�� �� �����ϴ�.");
            return;
        }

        if (TechLoader.Instance.researchedItems.Contains(trimmedName))
        {
            Debug.Log($"{trimmedName} ��(��) ���� �Ϸ��");

            DisableButtonCompletely();

        }
        else
        {
            Debug.Log($"{trimmedName} ��(��) ���� �������� ����");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) // R Ű�� ������ ���
        {
            foreach (string item in TechLoader.Instance.researchedItems)
            {
                Debug.Log($"[������] {item}");
            }
        }
    }


    public void test()
    {


        string trimmedName = itemName.Trim();

        if (!TechLoader.Instance.researchedItems.Contains(trimmedName))
        {
            TechLoader.Instance.researchedItems.Add(trimmedName);
            Debug.Log($"{trimmedName} ���� �Ϸ�");
        }

        DisableButtonCompletely();

        if (uiToClose != null)
            uiToClose.SetActive(false); // UI â ����
    }


    private void DisableButtonCompletely()
    {
        if (alreadyDisabled) return;
        alreadyDisabled = true;

        // ��ư ��Ȱ��ȭ
        if (button != null)
            button.interactable = false;

        // ���� ȿ�� ���� ����
        if (buttonImage != null)
            buttonImage.raycastTarget = false;

        if (buttonText != null)
            buttonText.raycastTarget = false; // TMP_Text��� TMP_Text�� ��ü
    }
}
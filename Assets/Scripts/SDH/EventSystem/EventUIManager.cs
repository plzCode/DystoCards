using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �̺�Ʈ UI�� �����ϴ� Ŭ����
/// 1���� ī�常 �����ְ� O/X ��ư���� �����ϴ� ���
/// </summary>
public class EventUIManager : MonoBehaviour
{
    [SerializeField] private GameObject eventChoiceUIPanel; // �̺�Ʈ UI Panel (��Ȱ��/Ȱ�� ��ȯ��)
    [SerializeField] private TMP_Text descriptionText;      // ī�� ������ ������ TMP_Text
    [SerializeField] private Button acceptButton;           // O ��ư
    [SerializeField] private Button rejectButton;           // X ��ư
    [SerializeField] private GameObject eventResultUIPanel; // �̺�Ʈ UI Panel (��Ȱ��/Ȱ�� ��ȯ��)
    [SerializeField] private TMP_Text resultText;           // ī�� ����� ������ TMP_Text
    [SerializeField] private Button closeButton;            // X ��ư

    private EventCardData currentCard; // ���� ī�� ������

    private void Start()
    {
        // ���� ���� �� ��Ȱ��ȭ
        eventChoiceUIPanel.SetActive(false);
        eventResultUIPanel.SetActive(false);

        // ��ư �̺�Ʈ�� Start���� �� ���� ����
        acceptButton.onClick.AddListener(AcceptCard);
        rejectButton.onClick.AddListener(RejectCard);
        closeButton.onClick.AddListener(CloseResult);

        TurnManager.Instance.RegisterPhaseAction(TurnPhase.EventDraw, () => OpenEventUI());
    }

    /// <summary>
    /// �̺�Ʈ UI�� ���� ī�� 1���� �������� ����
    /// </summary>
    private void OpenEventUI()
    {
        currentCard = EventFunctionManager.Instance.GetRandomCard();

        descriptionText.text = currentCard.description;

        UIManager.Instance.TogglePanel(eventChoiceUIPanel);

        Debug.Log($"[�̺�Ʈ] {currentCard.cardName} ����");
    }

    /// <summary>
    /// �̺�Ʈ UI �ݱ�
    /// </summary>
    private void CloseResult()
    {
        UIManager.Instance.TogglePanel(eventResultUIPanel);
        EventFunctionManager.Instance.Execute(currentCard.functionKey);
        TurnManager.Instance.MarkActionComplete();
    }

    /// <summary>
    /// O ��ư Ŭ�� �� ȣ��
    /// ī���� ��� ���� �� UI �ݱ�
    /// </summary>
    private void AcceptCard()
    {
        Debug.Log($"[{currentCard.cardName}] O ��ư ���õ�");

        UIManager.Instance.TogglePanel(eventChoiceUIPanel);
        resultText.text = currentCard.eventResult;
        UIManager.Instance.TogglePanel(eventResultUIPanel);
    }

    /// <summary>
    /// X ��ư Ŭ�� �� ȣ��
    /// Ư���� ������ ���ٸ� �׳� �ݱ�
    /// </summary>
    private void RejectCard()
    {
        Debug.Log($"[{currentCard.cardName}] X ��ư ���õ�");

        UIManager.Instance.TogglePanel(eventChoiceUIPanel);
    }
}

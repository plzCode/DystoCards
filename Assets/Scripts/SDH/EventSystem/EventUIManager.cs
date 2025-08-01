using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �̺�Ʈ UI�� �����ϴ� Ŭ����
/// 1���� ī�常 �����ְ� O/X ��ư���� �����ϴ� ���
/// </summary>
public class EventUIManager : MonoBehaviour
{
    [SerializeField] private GameObject eventUIPanel; // �̺�Ʈ UI Panel (��Ȱ��/Ȱ�� ��ȯ��)
    [SerializeField] private TMP_Text cardText;      // ī�� �̸��� ������ TMP_Text
    [SerializeField] private Button acceptButton;    // O ��ư
    [SerializeField] private Button rejectButton;    // X ��ư

    private EventCardData currentCard; // ���� ī�� ������

    private void Start()
    {
        eventUIPanel.SetActive(false); // ���� ���� �� ��Ȱ��ȭ

        // ��ư �̺�Ʈ�� Start���� �� ���� ����
        acceptButton.onClick.AddListener(AcceptCard);
        rejectButton.onClick.AddListener(RejectCard);

        TurnManager.Instance.RegisterPhaseAction(TurnPhase.EventDraw, () => OpenEventUI());
    }

    /// <summary>
    /// �̺�Ʈ UI�� ���� ī�� 1���� �������� ����
    /// </summary>
    private void OpenEventUI()
    {
        currentCard = EventFunctionManager.Instance.GetRandomCard();

        cardText.text = currentCard.description;

        UIManager.Instance.TogglePanel(eventUIPanel);

        Debug.Log($"[�̺�Ʈ] {currentCard.cardName} ����");
    }

    /// <summary>
    /// �̺�Ʈ UI �ݱ�
    /// </summary>
    private void CloseEventUI()
    {
        UIManager.Instance.TogglePanel(eventUIPanel);
    }

    /// <summary>
    /// O ��ư Ŭ�� �� ȣ��
    /// ī���� ��� ���� �� UI �ݱ�
    /// </summary>
    private void AcceptCard()
    {
        Debug.Log($"[{currentCard.cardName}] O ��ư ���õ�");

        EventFunctionManager.Instance.Execute(currentCard.functionKey);
        CloseEventUI();
    }

    /// <summary>
    /// X ��ư Ŭ�� �� ȣ��
    /// Ư���� ������ ���ٸ� �׳� �ݱ�
    /// </summary>
    private void RejectCard()
    {
        Debug.Log($"[{currentCard.cardName}] X ��ư ���õ�");

        CloseEventUI();
    }
}

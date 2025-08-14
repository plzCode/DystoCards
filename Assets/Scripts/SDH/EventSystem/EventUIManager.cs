using System.Collections;
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
        rejectButton.onClick.AddListener(TurnManager.Instance.MarkActionComplete);
        closeButton.onClick.AddListener(CloseResult);
        closeButton.onClick.AddListener(TurnManager.Instance.MarkActionComplete);

        TurnManager.Instance.RegisterPhaseAction(TurnPhase.EventDraw, () => OpenEventUI());
    }

    /// <summary>
    /// �̺�Ʈ UI�� ���� ī�� 1���� �������� ����
    /// </summary>
    private void OpenEventUI()
    {
        currentCard = EventFunctionManager.Instance.GetRandomCard();

        descriptionText.text = currentCard.description;

        AudioManager.Instance.PlaySFX("EventActivate");

        // Appear ȿ���� ��Ÿ����
        var dissolve = eventChoiceUIPanel.GetComponent<UI_Dissolve_Modify>();
        if (dissolve != null)
        {
            StartCoroutine(dissolve.Appear(true, true));
        }
        else
        {
            // Dissolve ��ũ��Ʈ ������ �׳� Ȱ��ȭ
            UIManager.Instance.TogglePanel(eventChoiceUIPanel);
        }
    }

    /// <summary>
    /// �̺�Ʈ UI �ݱ�
    /// </summary>
    private void CloseResult()
    {
        EventFunctionManager.Instance.Execute(currentCard.functionKey);

        AudioManager.Instance.PlaySFX("EventActivate");

        // Dissolve ȿ���� �������
        var dissolve = eventResultUIPanel.GetComponent<UI_Dissolve_Modify>();
        if (dissolve != null)
        {
            StartCoroutine(dissolve.Vanish(true, true));
        }
        else
        {
            // Dissolve ��ũ��Ʈ ������ �׳� ��Ȱ��ȭ
            UIManager.Instance.TogglePanel(eventResultUIPanel);
        }

        Recorder.Instance.RecordEvent(currentCard.eventResult, TurnManager.Instance.TurnCount);
    }

    private void AcceptCard()
    {
        StartCoroutine(AcceptCardRoutine());
    }

    /// <summary>
    /// O ��ư Ŭ�� �� ȣ��
    /// ī���� ��� ���� �� UI �ݱ�
    /// </summary>
    private IEnumerator AcceptCardRoutine()
    {
        AudioManager.Instance.PlaySFX("EventActivate");

        // Dissolve ȿ���� �������
        var dissolveVanish = eventChoiceUIPanel.GetComponent<UI_Dissolve_Modify>();
        if (dissolveVanish != null)
            yield return StartCoroutine(dissolveVanish.Vanish(true, true));
        else
            UIManager.Instance.TogglePanel(eventChoiceUIPanel);

        // ��� �ؽ�Ʈ ����
        resultText.text = currentCard.eventResult;

        // ��� ��� (0.5�� ����)
        yield return new WaitForSeconds(0.5f);

        AudioManager.Instance.PlaySFX("EventActivate");

        // Appear ȿ���� ��Ÿ����
        var dissolveAppear = eventResultUIPanel.GetComponent<UI_Dissolve_Modify>();
        if (dissolveAppear != null)
            yield return StartCoroutine(dissolveAppear.Appear(true, true));
        else
            UIManager.Instance.TogglePanel(eventResultUIPanel);
    }

    /// <summary>
    /// X ��ư Ŭ�� �� ȣ��
    /// Ư���� ������ ���ٸ� �׳� �ݱ�
    /// </summary>
    private void RejectCard()
    {
        AudioManager.Instance.PlaySFX("EventActivate");

        // Dissolve ȿ���� �������
        var dissolve = eventChoiceUIPanel.GetComponent<UI_Dissolve_Modify>();
        if (dissolve != null)
        {
            StartCoroutine(dissolve.Vanish(true, true));
        }
        else
        {
            // Dissolve ��ũ��Ʈ ������ �׳� ��Ȱ��ȭ
            UIManager.Instance.TogglePanel(eventChoiceUIPanel);
        }
    }
}

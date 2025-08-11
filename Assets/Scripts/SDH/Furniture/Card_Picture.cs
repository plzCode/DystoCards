using UnityEngine;

/// <summary>
/// �׸� ī�� Ŭ����
/// �Ϸ簡 ���� �� �� �� ��� Human ������Ʈ�� ���� �ǰ��� ȸ��
/// </summary>
public class Card_Picture : MonoBehaviour
{
    [SerializeField] private float mentalRecoveryAmout = 1f; // ȸ���� ���� �ǰ���

    private void Start()
    {
        // TurnManager�� DayEnd �ܰ迡 Use �޼��带 ���
        TurnManager.Instance.RegisterPhaseAction(TurnPhase.DayEnd, () => Use());
    }

    // �׸� ī���� ȿ���� �����ϴ� �Լ�
    private void Use()
    {
        // �� �� ��� Human ������Ʈ ������Ʈ ã��
        Human[] humans = Object.FindObjectsByType<Human>(FindObjectsSortMode.None);

        foreach (Human human in humans)
        {
            Debug.Log("Human ������Ʈ �߰�: " + human.gameObject.name); // �߰� �α� ���
            if (human != null)
                human.RecoverMentalHealth(mentalRecoveryAmout); // ���� �ǰ� ȸ�� ȣ��
        }
    }
}

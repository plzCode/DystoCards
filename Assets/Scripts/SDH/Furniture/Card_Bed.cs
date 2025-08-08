using UnityEngine;

/// <summary>
/// ħ�� ������ �ϴ� ī�� Ŭ����
/// �Ϸ簡 ���� �� ħ�� ���� �ִ� ĳ������ ���¹̳��� ȸ��
/// </summary>
public class Card_Bed : MonoBehaviour
{
    [SerializeField] private float staminaRecoveryAmout = 1f; // ȸ���� ���¹̳� ��

    private void Start()
    {
        // TurnManager�� DayEnd �ܰ迡�� Use() �Լ� ���� ���
        TurnManager.Instance.RegisterPhaseAction(TurnPhase.DayEnd, () => Use());
    }

    // ħ�� ��� �� ȣ��Ǵ� �Լ�
    private void Use()
    {
        // �ڽ��� �ϳ� �̻� �ִ��� Ȯ�� (ħ�� ���� ����� �ִ��� Ȯ��)
        if (transform.childCount > 0)
        {
            Transform firstChild = transform.GetChild(0); // ù ��° �ڽ� ��������

            // �ڽĿ� Human ������Ʈ�� �ִ��� Ȯ��
            Human human = firstChild.GetComponent<Human>();
            if (human != null)
            {
                // ���¹̳� ȸ�� �Լ� ȣ��
                human.RecoverStamina(staminaRecoveryAmout);
            }
        }
    }
}

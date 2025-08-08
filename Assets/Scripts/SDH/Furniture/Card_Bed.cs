using UnityEngine;

public class Card_Bed : MonoBehaviour
{
    [SerializeField] private float staminaRecoveryAmout = 1f;

    private void Start()
    {
        TurnManager.Instance.RegisterPhaseAction(TurnPhase.DayEnd, () => Use());
    }

    private void Use()
    {
        // �ڽ��� �ϳ� �̻� �ִ��� Ȯ��
        if (transform.childCount > 0)
        {
            Transform firstChild = transform.GetChild(0);

            // ��: �ڽ� ������Ʈ�� Human ��ũ��Ʈ�� �پ� �ִٸ� �۾� ����
            Human human = firstChild.GetComponent<Human>();
            if (human != null)
            {
                human.RecoverStamina(staminaRecoveryAmout);
            }
        }
    }
}

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
        // 자식이 하나 이상 있는지 확인
        if (transform.childCount > 0)
        {
            Transform firstChild = transform.GetChild(0);

            // 예: 자식 오브젝트에 Human 스크립트가 붙어 있다면 작업 실행
            Human human = firstChild.GetComponent<Human>();
            if (human != null)
            {
                human.RecoverStamina(staminaRecoveryAmout);
            }
        }
    }
}

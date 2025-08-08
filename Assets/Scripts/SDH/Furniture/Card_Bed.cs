using UnityEngine;

/// <summary>
/// 침대 역할을 하는 카드 클래스
/// 하루가 끝날 때 침대 위에 있는 캐릭터의 스태미나를 회복
/// </summary>
public class Card_Bed : MonoBehaviour
{
    [SerializeField] private float staminaRecoveryAmout = 1f; // 회복할 스태미나 양

    private void Start()
    {
        // TurnManager에 DayEnd 단계에서 Use() 함수 실행 등록
        TurnManager.Instance.RegisterPhaseAction(TurnPhase.DayEnd, () => Use());
    }

    // 침대 사용 시 호출되는 함수
    private void Use()
    {
        // 자식이 하나 이상 있는지 확인 (침대 위에 사람이 있는지 확인)
        if (transform.childCount > 0)
        {
            Transform firstChild = transform.GetChild(0); // 첫 번째 자식 가져오기

            // 자식에 Human 컴포넌트가 있는지 확인
            Human human = firstChild.GetComponent<Human>();
            if (human != null)
            {
                // 스태미나 회복 함수 호출
                human.RecoverStamina(staminaRecoveryAmout);
            }
        }
    }
}

using UnityEngine;

/// <summary>
/// 그림 카드 클래스
/// 하루가 끝날 때 씬 내 모든 Human 오브젝트의 정신 건강을 회복
/// </summary>
public class Card_Picture : MonoBehaviour
{
    [SerializeField] private float mentalRecoveryAmout = 1f; // 회복할 정신 건강량

    private void Start()
    {
        // TurnManager의 DayEnd 단계에 Use 메서드를 등록
        TurnManager.Instance.RegisterPhaseAction(TurnPhase.DayEnd, () => Use());
    }

    // 그림 카드의 효과를 적용하는 함수
    private void Use()
    {
        // 씬 내 모든 Human 컴포넌트 오브젝트 찾기
        Human[] humans = Object.FindObjectsByType<Human>(FindObjectsSortMode.None);

        foreach (Human human in humans)
        {
            Debug.Log("Human 오브젝트 발견: " + human.gameObject.name); // 발견 로그 출력
            if (human != null)
                human.RecoverMentalHealth(mentalRecoveryAmout); // 정신 건강 회복 호출
        }
    }
}

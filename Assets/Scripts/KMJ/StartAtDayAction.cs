using System.Collections;
using UnityEngine;

public class StartAtDayAction : MonoBehaviour
{
    private IEnumerator Start()
    {
        // TurnManager.Start() 이후 한 프레임 대기
        yield return null;

        if (TurnManager.Instance.CurrentPhase == TurnPhase.EventDraw)
        {
            // EventDraw → Explore → DayAction 로 두 단계 넘기려면 2번 호출
            TurnManager.Instance.NextPhase();   // EventDraw ➜ DayAction
        }
    }
}
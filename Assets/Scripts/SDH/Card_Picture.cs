using UnityEngine;

public class Card_Picture : MonoBehaviour
{
    private void Start()
    {
        TurnManager.Instance.RegisterPhaseAction(TurnPhase.DayAction, () => FindAllHumans());
    }

    private void FindAllHumans()
    {
        Human[] humans = Object.FindObjectsByType<Human>(FindObjectsSortMode.None);

        foreach (Human human in humans)
        {
            Debug.Log("Human 오브젝트 발견: " + human.gameObject.name);
            if (human != null)
                human.RecoverMentalHealth(1f);
        }
    }
}

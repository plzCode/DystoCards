using UnityEngine;

public class Card_Picture : MonoBehaviour
{
    [SerializeField] private float mentalRecoveryAmout = 1f;

    private void Start()
    {
        TurnManager.Instance.RegisterPhaseAction(TurnPhase.DayEnd, () => Use());
    }

    private void Use()
    {
        Human[] humans = Object.FindObjectsByType<Human>(FindObjectsSortMode.None);

        foreach (Human human in humans)
        {
            Debug.Log("Human 오브젝트 발견: " + human.gameObject.name);
            if (human != null)
                human.RecoverMentalHealth(mentalRecoveryAmout);
        }
    }
}

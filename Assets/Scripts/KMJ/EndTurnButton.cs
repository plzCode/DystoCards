using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class EndTurnButton : MonoBehaviour
{
    [SerializeField] private ConfirmPopup popup;   // Inspector에서 drag

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (CharacterTaskRunner.Instance.HasActionableCharacter())
        {
            popup.Open("There are still characters who can act. End turn?",
                       () => TurnManager.Instance.NextPhase());
        }
        else
        {
            TurnManager.Instance.NextPhase();
        }
    }
}
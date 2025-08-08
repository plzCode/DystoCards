using UnityEngine;

public class PhaseInputGate : MonoBehaviour
{
    private TurnPhase lastPhase;

    private void Start()
    {
        lastPhase = TurnManager.Instance.CurrentPhase;
        UpdateGate(lastPhase);
    }

    private void Update()
    {
        var cur = TurnManager.Instance.CurrentPhase;
        if (cur != lastPhase)
        {
            lastPhase = cur;
            UpdateGate(cur);
        }
    }

    private void UpdateGate(TurnPhase phase)
    {
        InputGate.Enabled = (phase == TurnPhase.DayAction);
        Debug.Log($"<color=orange>[Gate] {phase} → Input {(InputGate.Enabled ? "ON" : "OFF")}</color>");
    }
}

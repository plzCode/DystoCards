using UnityEngine;
public enum TilePhase { Locked, Frontier, Unlocked }

public class TileState : MonoBehaviour
{
    [SerializeField] private SpriteRenderer fog;
    [SerializeField] private SpriteRenderer hint;

    public TilePhase Current { get; private set; } = TilePhase.Locked;

    public void SetState(TilePhase state)
    {
        Current = state;
        fog.enabled = (state == TilePhase.Locked);
        hint.enabled = (state == TilePhase.Frontier);
    }

    private void OnMouseDown()
    {
        if (Current == TilePhase.Frontier)
            MapManager.Instance.TryUnlock(this);
    }
}
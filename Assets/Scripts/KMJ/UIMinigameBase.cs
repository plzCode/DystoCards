using System;
using UnityEngine;

public abstract class UIMinigameBase : MonoBehaviour
{
    Action<bool> _onDone;

    public void Begin(Action<bool> onDone)
    {
        _onDone = onDone;
        OnStartGame();
    }

    protected abstract void OnStartGame();

    protected void Complete(bool success)
    {
        _onDone?.Invoke(success);
        _onDone = null;
    }
}

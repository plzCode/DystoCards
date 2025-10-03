using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MinigameRunner
{
    static Action<bool> onDone;
    static string loaded;

    public static bool IsRunning => onDone != null;

    public static void Run(string sceneName, Action<bool> done)
    {
        if (IsRunning) return;               // 중복 방지
        onDone = done;
        loaded = sceneName;

        TurnBridge.BeginAction();            // 시작 시 true
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    public static void Complete(bool success)
    {
        if (!IsRunning) return;
        var cb = onDone; onDone = null;
        var sc = loaded; loaded = null;

        void Finish()
        {
            cb?.Invoke(success);
            TurnBridge.MarkComplete();       // 끝나면 false (MarkActionComplete)
        }

        if (!string.IsNullOrEmpty(sc))
            SceneManager.UnloadSceneAsync(sc).completed += _ => Finish();
        else
            Finish();
    }
}
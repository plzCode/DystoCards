using System;
using UnityEngine;

public static class MinigameUIRunner
{
    public static GameObject Show(GameObject prefab, Transform uiRoot, Action<bool> onDone)
    {
        if (prefab == null || uiRoot == null) { Debug.LogError("[MiniUI] prefab/uiRoot null"); return null; }

        TurnBridge.BeginAction();               // 이벤트 시작 (isActionRunning = true)

        var cg = uiRoot.GetComponent<CanvasGroup>();

        if (cg)
        {
            cg.alpha = 1f;                 // 딤 보이기
            cg.blocksRaycasts = true;      // 입력 차단
            cg.interactable = true;
        }

        var go = UnityEngine.Object.Instantiate(prefab, uiRoot);
        var ui = go.GetComponent<UIMinigameBase>();
        if (ui == null) { Debug.LogError("[MiniUI] UIMinigameBase가 필요합니다."); return go; }

        ui.Begin(success =>
        {
            UnityEngine.Object.Destroy(go);

            if (cg)
            {
                cg.alpha = 0f;             // 딤 숨기기
                cg.blocksRaycasts = false; // 입력 허용
                cg.interactable = false;
            }

            onDone?.Invoke(success);
            TurnBridge.MarkComplete();          // 이벤트 끝 (MarkActionComplete)
        });

        return go;
    }
}

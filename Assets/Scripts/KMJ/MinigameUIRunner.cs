using System.Collections.Generic;
using UnityEngine;

public static class MinigameUIRunner
{
    static readonly List<Collider2D> s_disabled = new();
    static int s_cardLayer = -1;

    static void LockCardColliders(bool on)
    {
        if (on)
        {
            s_disabled.Clear();
            if (s_cardLayer < 0) s_cardLayer = LayerMask.NameToLayer("Card");
            foreach (var col in Object.FindObjectsOfType<Collider2D>())
            {
                if (!col || !col.enabled) continue;
                if (s_cardLayer >= 0 && col.gameObject.layer != s_cardLayer) continue;
                col.enabled = false;
                s_disabled.Add(col);
            }
        }
        else
        {
            foreach (var col in s_disabled) if (col) col.enabled = true;
            s_disabled.Clear();
        }
    }

    public static GameObject Show(GameObject prefab, Transform uiRoot, System.Action<bool> onDone)
    {
        if (prefab == null || uiRoot == null) { Debug.LogError("[MiniUI] prefab/uiRoot null"); return null; }

        TurnBridge.BeginAction();

        var cg = uiRoot.GetComponent<CanvasGroup>();
        if (cg)
        {
            cg.alpha = 1f;
            cg.blocksRaycasts = true;
            cg.interactable = true;
        }
        (uiRoot as RectTransform)?.SetAsLastSibling(); // 항상 맨 위

        // ★ 카드 드래그 잠금
        LockCardColliders(true);

        var go = Object.Instantiate(prefab, uiRoot);
        var ui = go.GetComponent<UIMinigameBase>();
        if (ui == null) { Debug.LogError("[MiniUI] UIMinigameBase가 필요합니다."); return go; }

        ui.Begin(success =>
        {
            Object.Destroy(go);

            // ★ 카드 드래그 복구
            LockCardColliders(false);

            if (cg)
            {
                cg.alpha = 0f;
                cg.blocksRaycasts = false;
                cg.interactable = false;
            }

            onDone?.Invoke(success);
            TurnBridge.MarkComplete();
        });

        return go;
    }
}
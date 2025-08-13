using System.Reflection;
using UnityEngine;

public static class TurnBridge
{
    static BindingFlags BF = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    public static void BeginAction()
    {
        var tm = TurnManager.Instance; if (!tm) return;
        var f = tm.GetType().GetField("isActionRunning", BF);
        if (f != null) f.SetValue(tm, true);
    }

    public static void MarkComplete()
    {
        var tm = TurnManager.Instance; if (!tm) return;

        var m = tm.GetType().GetMethod("MarkActionComplete", BF);
        if (m != null) { m.Invoke(tm, null); return; }

        var f = tm.GetType().GetField("isActionRunning", BF);
        if (f != null) f.SetValue(tm, false);
    }
}

using System.Collections.Generic;
using UnityEngine;

public class ResourceBank : MonoBehaviour
{
    public static ResourceBank Instance { get; private set; }
    void Awake() { if (Instance == null) Instance = this; }

    readonly Dictionary<string, int> stock = new();

    public void Add(string id, int amount)
    {
        if (string.IsNullOrEmpty(id) || amount == 0) return;
        stock.TryGetValue(id, out var cur);
        stock[id] = cur + amount;
        Debug.Log($"[Resource] +{amount} {id} → {stock[id]}");
    }

    public bool TryPay(string id, int amount)
    {
        if (string.IsNullOrEmpty(id) || amount <= 0) return false;
        stock.TryGetValue(id, out var cur);
        if (cur < amount) return false;
        stock[id] = cur - amount;
        Debug.Log($"[Resource] -{amount} {id} → {stock[id]}");
        return true;
    }

    public int Get(string id) => stock.TryGetValue(id, out var v) ? v : 0;
}
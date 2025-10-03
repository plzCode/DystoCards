using UnityEngine;
public static class ResourceManager
{
    public static void Add(ResourceType t, int v)
        => Debug.Log($"[Res] {t} +{v}");
}
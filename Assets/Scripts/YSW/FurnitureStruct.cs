using UnityEngine;

public abstract class FurnitureStruct : MonoBehaviour
{
    public abstract void Initialize();
    public abstract void Use();

    public void Start()
    {
        Use();
    }
}

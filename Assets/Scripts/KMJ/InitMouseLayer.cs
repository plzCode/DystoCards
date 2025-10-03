using UnityEngine;

[RequireComponent(typeof(MouseInput))]
public class InitMouseLayer : MonoBehaviour
{
    void Awake()
    {
        var mi = GetComponent<MouseInput>();
        if (mi.cardLayer == 0)              // Nothing 이면
            mi.cardLayer = LayerMask.GetMask("Card");

        if (mi.interactableLayerMask == 0)  // 필요시
            mi.interactableLayerMask = LayerMask.GetMask("Interactable");
    }
}

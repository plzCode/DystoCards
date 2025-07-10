using UnityEngine;

public class Chair : FurnitureStruct
{
    [SerializeField] FurnitureCardData cardData;
    [SerializeField] private float staminaRecoveryAmout = 1f;
    [SerializeField] private float sleepDuration = 2.0f; // ���� �ð�
    [SerializeField] GameObject interactHuman;
    public override void Initialize()
    {

    }

    public override void Use()
    {
        Debug.Log("Called Use() on Chair");
    }


}

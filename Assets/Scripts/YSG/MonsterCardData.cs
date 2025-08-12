using UnityEngine;

public enum MonsterActionType
{
    Default,
    ItemSteal
}

[System.Serializable]
public class DropItem
{
    public CardData item;
    [Range(0, 100)] public int chance = 100;
}

[CreateAssetMenu(menuName = "Cards/11.MonsterCard", order = 12)]
public class MonsterCardData : CharacterCardData
{
       [Header("Monster Attributes")]
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private MonsterActionType act;

    [Header("Drop Attributes")]
    [SerializeField] private DropItem[] dropList;

    public float MoveSpeed
    {
        get => moveSpeed;
        set
        {
            if (moveSpeed != value)
            {
                moveSpeed = value;
                RaiseDataChanged();
            }
        }
    }

    public MonsterActionType Act
    {
        get => act;
        set
        {
            if (act != value)
            {
                act = value;
                RaiseDataChanged();
            }
        }
    }

    public DropItem[] Drops
    {
        get => dropList;
        set
        {
            if (dropList != value)
            {
                dropList = value;
                RaiseDataChanged();
            }
        }
    }

    public override CardData Clone()
    {
        var clone = CreateInstance<MonsterCardData>();

        clone.cardId = this.cardId;
        clone.cardName = this.cardName;
        clone.cardType = this.cardType;
        clone.cardImage = this.cardImage;
        clone.size = this.size;
        clone.characterType = this.characterType;

        clone.MaxHealth = this.MaxHealth;
        clone.AttackPower = this.AttackPower;
        clone.DefensePower = this.DefensePower;

        clone.moveSpeed = this.moveSpeed;
        clone.act = this.act;

        clone.dropList = this.dropList;

        return clone;
    }
}
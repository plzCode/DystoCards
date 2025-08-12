using UnityEngine;

public enum MonsterActionType
{
    Default,
    ItemSteal
}

[CreateAssetMenu(menuName = "Cards/11.MonsterCard", order = 12)]
public class MonsterCardData : CharacterCardData
{
    [Header("Monster Attributes")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private MonsterActionType act;

    [Header("Drop Attributes")]
    [SerializeField] private int dropCount;
    [SerializeField] private CardData[] dropList;

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

    public CardData[] Drops
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

        clone.moveSpeed = this.MoveSpeed;
        clone.act = this.act;

        clone.dropCount = this.dropCount;
        clone.dropList = this.dropList;

        return clone;
    }
}
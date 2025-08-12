using UnityEngine;

public enum MonsterActionType
{
    Default,
    ItemSteal
}

[CreateAssetMenu(menuName = "Cards/11.MonsterCardData", order = 12)]
public class MonsterCardData : CharacterCardData
{
    [Header("Monster Attributes")]
    [SerializeField] private CardData[] drops;
    public MonsterActionType act;

    public CardData[] Drops
    {
        get => drops;
        set
        {
            if (drops != value)
            {
                drops = value;
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

        clone.drops = this.drops;
        clone.act = this.act;

        return clone;
    }
}
using UnityEngine;

public enum MonsterActionType
{
    Default,
    Steal,
    Robbery,
}

[System.Serializable]
public class DropItem
{
    public CardData item;
    [Range(0, 100)] public int chance = 100;

    [Min(0)] public int minCount = 0;
    [Min(1)] public int maxCount = 1;

    public bool isOnly = false;

    public void Validate()
    {
        if (isOnly)
        {
            minCount = 0;
            maxCount = 1;
        }

        if (maxCount < minCount) maxCount = minCount;
    }
}

[CreateAssetMenu(menuName = "Cards/11.MonsterCard", order = 12)]
public class MonsterCardData : CharacterCardData
{
    [Header("Monster Attributes")]
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private MonsterActionType act;

    [Header("Spawn Attributes")]
    [SerializeField][Min(0)] private int spawnTurn = 0;
    [SerializeField][Range(0, 100)] private int spawnChance = 0;

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

    public int SpawnTurn
    {
        get => spawnTurn;
        set
        {
            if (spawnTurn != value)
            {
                spawnTurn = Mathf.Max(0, value);
                RaiseDataChanged();
            }
        }
    }

    public int SpawnChance
    {
        get => spawnChance;
        set
        {
            if (spawnChance != value)
            {
                spawnChance = Mathf.Clamp(value, 0, 100);
                RaiseDataChanged();
            }
        }
    }

    public DropItem[] DropList
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

        clone.spawnTurn = this.spawnTurn;
        clone.spawnChance = this.spawnChance;

        if (this.dropList != null)
        {
            clone.dropList = new DropItem[this.dropList.Length];
            for (int i = 0; i < this.dropList.Length; i++)
            {
                if (this.dropList[i] != null)
                {
                    var drop = new DropItem
                    {
                        item = this.dropList[i].item,
                        chance = this.dropList[i].chance,
                        minCount = this.dropList[i].minCount,
                        maxCount = this.dropList[i].maxCount,
                        isOnly = this.dropList[i].isOnly
                    };
                    drop.Validate();
                    clone.dropList[i] = drop;
                }
            }
        }

        return clone;
    }
}

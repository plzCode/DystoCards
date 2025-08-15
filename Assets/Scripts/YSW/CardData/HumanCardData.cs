using UnityEngine;

[CreateAssetMenu(menuName = "Cards/07.HumanCard", order = 8)]
public class HumanCardData : CharacterCardData
{
    [Header("Human Attributes")]
    [SerializeField] private float max_mental_health = 10f;
    [SerializeField] private float current_mental_health = 10f;
    [SerializeField] private float max_hunger = 5f;
    [SerializeField] private float current_hunger = 5f;
    [SerializeField] private float stamina = 5f;
    [SerializeField] private float max_stamina = 5f;
    [SerializeField] private float consume_hunger = 1f;

    public float MaxMentalHealth
    {
        get => max_mental_health;
        set
        {
            if (max_mental_health != value)
            {
                max_mental_health = value;
                RaiseDataChanged();
            }
        }
    }
    public float CurrentMentalHealth
    {
        get => current_mental_health;
        set
        {
            if(current_mental_health != value)
            {
                current_mental_health = value;
                RaiseDataChanged();
            }
        }
    }

    public float MaxHunger
    {
        get => max_hunger;
        set
        {
            if (max_hunger != value)
            {
                max_hunger = value;
                RaiseDataChanged();
            }
        }
    }
    public float CurrentHunger
    {
        get => current_hunger;
        set
        {
            if(current_hunger != value)
            {
                current_hunger = value;
                RaiseDataChanged();
            }
        }
    }

    public float Stamina
    {
        get => stamina;
        set
        {
            if (stamina != value)
            {
                stamina = value;
                RaiseDataChanged();
            }
        }
    }

    public float MaxStamina
    {
        get => max_stamina;
        set
        {
            if(max_stamina != value)
            {
                max_stamina = value;
                RaiseDataChanged();
            }
        }
    }

    public float ConsumeHunger
    {
        get => consume_hunger;
        set
        {
            if (consume_hunger != value)
            {
                consume_hunger = value;
                RaiseDataChanged();
            }
        }
    }


    public override CardData Clone()
    {
        // HumanCardData 타입으로 인스턴스 생성
        var clone = CreateInstance<HumanCardData>();

        // 부모 필드 복사 (CharacterCardData → CardData 까지 모두 포함)
        clone.cardId = this.cardId;
        clone.cardName = this.cardName;
        clone.cardType = this.cardType;
        clone.cardImage = this.cardImage;
        clone.size = this.size;
        clone.characterType = this.characterType;

        clone.MaxHealth = this.MaxHealth;
        clone.AttackPower = this.AttackPower;
        clone.DefensePower = this.DefensePower;

        // HumanCardData 고유 필드 복사
        clone.max_mental_health = this.max_mental_health;
        clone.max_hunger = this.max_hunger;
        clone.stamina = this.stamina;
        clone.consume_hunger = this.consume_hunger;

        return clone;
    }
}

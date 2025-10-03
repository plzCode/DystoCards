using UnityEngine;

public class CharacterCardData : CardData
{
    public CharacterType characterType;

    [Header("Character Attributes")]
    [SerializeField] private float max_health = 20f;
    [SerializeField] private float current_health = 20f; // 현재 체력, 필요시 사용
    [SerializeField] private float attack_power = 2f;
    [SerializeField] private float defense_power = 1f;

    public CharacterType GetCharacterType()
    {
        return characterType;
    }

    public event System.Action OnDataChanged;

    public float MaxHealth
    {
        get => max_health;
        set
        {
            if (max_health != value)
            {
                max_health = value;
                OnDataChanged?.Invoke();
            }
        }
    }

    public float CurrentHealth
    {
        get => current_health;
        set
        {
            if (current_health != value)
            {
                current_health = value;
                OnDataChanged?.Invoke();
            }
        }
    }

    public float AttackPower
    {
        get => attack_power;
        set
        {
            if (attack_power != value)
            {
                attack_power = value;
                OnDataChanged?.Invoke();
            }
        }
    }

    public float DefensePower
    {
        get => defense_power;
        set
        {
            if (defense_power != value)
            {
                defense_power = value;
                OnDataChanged?.Invoke();
            }
        }
    }
    protected void RaiseDataChanged()
    {
        OnDataChanged?.Invoke();
    }

    // Clone 메서드 오버라이드
    public override CardData Clone()
    {
        CharacterCardData clone = ScriptableObject.CreateInstance<CharacterCardData>();

        // 부모 클래스 필드 복사
        clone.cardId = this.cardId;
        clone.cardName = this.cardName;
        clone.cardImage = this.cardImage;
        clone.cardType = this.cardType;
        clone.description = this.description;
        clone.size = this.size;

        // 자식 클래스 필드 복사
        clone.max_health = this.max_health;
        clone.attack_power = this.attack_power;
        clone.defense_power = this.defense_power;
        clone.characterType = this.characterType;

        return clone;
    }

}
public enum CharacterType
{
    Human,
    NPC,
    Monster,
}
using UnityEngine;

public class CharacterCardData : CardData
{
    public CharacterType characterType;

    [Header("Character Attributes")]
    public float max_health = 20f;
    public float attack_power = 2f;
    public float defense_power = 1f;

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

}
public enum CharacterType
{
    Human,
    NPC,
    Monster,
}
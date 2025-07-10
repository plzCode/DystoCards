using UnityEngine;

public class CharacterCardData : CardData
{
    public CharacterType characterType;

    [Header("Character Attributes")]
    public float max_health = 20f;
    public float attack_power = 2f;
    public float defense_power = 1f;
    
}
public enum CharacterType
{
    Human,
    NPC,
    Monster,
}
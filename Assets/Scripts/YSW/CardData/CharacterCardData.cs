using UnityEngine;

[CreateAssetMenu(menuName = "Cards/07.CharacterCard", order = 8)]
public class CharacterCardData : CardData
{
    public CharacterType characterType;

    [Header("Character Attributes")]
    public float max_health = 20f;
    public float current_health = 20f;
    public float attack_power = 2f;
    public float defense_power = 1f;
    public float max_hunger = 5f;
    
}
public enum CharacterType
{
    Human,
    NPC,
    Monster,
}
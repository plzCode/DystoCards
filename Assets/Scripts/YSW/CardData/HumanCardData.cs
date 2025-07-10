using UnityEngine;

[CreateAssetMenu(menuName = "Cards/07.HumanCard", order = 8)]
public class HumanCardData : CharacterCardData
{
    public float max_mental_health = 10f;
    public float max_hunger = 5f;
    public float stamina = 5f;
    public float consume_hunger = 1f;
}

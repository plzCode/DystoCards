using UnityEngine;

[CreateAssetMenu(menuName = "Cards/07.HumanCard", order = 8)]
public class HumanCardData : CharacterCardData
{
    public float max_mental_health = 10f;
    public float max_hunger = 5f;
    public float stamina = 5f;
    public float consume_hunger = 1f;

    public override CardData Clone()
    {
        // HumanCardData Ÿ������ �ν��Ͻ� ����
        var clone = CreateInstance<HumanCardData>();

        // �θ� �ʵ� ���� (CharacterCardData �� CardData ���� ��� ����)
        clone.cardId = this.cardId;
        clone.cardName = this.cardName;
        clone.cardType = this.cardType;
        clone.cardImage = this.cardImage;
        clone.size = this.size;
        clone.characterType = this.characterType;

        clone.MaxHealth = this.MaxHealth;
        clone.AttackPower = this.AttackPower;
        clone.DefensePower = this.DefensePower;

        // HumanCardData ���� �ʵ� ����
        clone.max_mental_health = this.max_mental_health;
        clone.max_hunger = this.max_hunger;
        clone.stamina = this.stamina;
        clone.consume_hunger = this.consume_hunger;

        return clone;
    }
}

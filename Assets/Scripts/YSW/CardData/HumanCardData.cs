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

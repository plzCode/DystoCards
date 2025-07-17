using UnityEngine;

public class TestCard : Character
{
    [Header("Ä«µå ½ºÅÈ")]
    public float maxHealth;
    public float attackPower;
    public float defensePower;

    public CardData[] drops;

    private void Awake()
    {
        charData = GetComponent<Card2D>().cardData as CharacterCardData;

        if (charData != null)
        {
            maxHealth = charData.max_health;
            currentHealth = charData.max_health;
            attackPower = charData.attack_power;
            defensePower = charData.defense_power;
        }
    }

    public override void TakeDamage(float _damage)
    {
        currentHealth -= _damage;

        if (currentHealth <= 0)
            Die();
    }

    public override void Die()
    {
        Debug.Log(name + " »ç¸Á");

        Destroy(gameObject);
    }
}

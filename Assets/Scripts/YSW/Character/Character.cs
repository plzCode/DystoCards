using UnityEngine;

public class Character : MonoBehaviour
{
    public CharacterCardData charData;

    public float currentHealth;

    public void Initialize(CharacterCardData data)
    {
        charData = (CharacterCardData)data.Clone();
        currentHealth = data.MaxHealth;
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - amount);
        Debug.Log($"{charData.cardName} took {amount} damage. HP: {currentHealth}/{charData.MaxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(float amount)
    {
        currentHealth = Mathf.Min(charData.MaxHealth, currentHealth + amount);
    }
    
    #region 전투 시스템 정해지면 수정 필요
    public virtual void Die()
    {
        var card = GetComponent<Card2D>();
        if (card != null)
            CardManager.Instance.DestroyCard(card);
    }
    public virtual void ChooseTarget()
    {

    }
    #endregion

    public virtual void Attack(Character target)
    {
        float damage = charData.AttackPower - target.charData.DefensePower;
        if (damage > 0)
        {
            target.TakeDamage(damage);
            //Debug.Log($"{charData.cardName} attacked {target.charData.cardName} for {damage} damage.");
        }
        else
        {
            Debug.Log($"{charData.cardName}'s attack was ineffective against {target.charData.cardName}.");
        }
    }
}

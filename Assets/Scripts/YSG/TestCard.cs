using System.Collections;
using UnityEngine;

public class TestCard : Card2D
{
    private SpriteRenderer sr;

    [Header("Ä«µå ½ºÅÈ")]
    public float maxHealth;
    public float currentHealth;
    public float attackPower;
    public float defensePower;

    public CardData[] drops;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        CharacterCardData characterData = cardData as CharacterCardData;
        if (characterData != null)
        {
            maxHealth = characterData.max_health;
            currentHealth = characterData.max_health;
            attackPower = characterData.attack_power;
            defensePower = characterData.defense_power;
        }
    }

    public IEnumerator AttackEffect(TestCard target)
    {
        Vector3 originPos = transform.position;
        Vector3 targetPos = target.transform.position;
        Vector3 midPos = Vector3.Lerp(originPos, targetPos, 0.8f);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 10;
            transform.position = Vector3.Lerp(originPos, midPos, t);
            yield return null;
        }

        yield return target.HitEffect();

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 10;
            transform.position = Vector3.Lerp(midPos, originPos, t);
            yield return null;
        }

        Debug.Log($"{name}°¡ {target.name}¸¦ °ø°Ý!");

        target.TakeDamage(attackPower);
    }

    public IEnumerator HitEffect()
    {
        if (sr == null) yield break;

        Color originalColor = sr.color;

        for (int i = 0; i < 2; i++)
        {
            sr.color = Color.clear;
            yield return new WaitForSeconds(0.1f);
            sr.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void TakeDamage(float _damage)
    {
        currentHealth -= _damage;

        if (currentHealth <= 0)
            Die();
    }

    public void Die()
    {
        Debug.Log(name + " »ç¸Á");

        Destroy(gameObject);
    }
}

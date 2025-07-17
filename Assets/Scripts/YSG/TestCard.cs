using System.Collections;
using UnityEngine;

public class TestCard : Character
{
    [Header("Ä«µå ½ºÅÈ")]
    public float maxHealth;
    public float attackPower;
    public float defensePower;

    public CardData[] drops;

    private float moveDistance = 1;
    private float moveDuration = 0.5f;

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

    private void Start()
    {
        StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(0.5f, 2f);
            yield return new WaitForSeconds(waitTime);

            Human target = BattleManager.Instance.humans[Random.Range(0, BattleManager.Instance.humans.Count)];
            if (target == null) continue;

            Vector3 startPos = transform.position;
            Vector3 dir = (target.transform.position - startPos).normalized;
            Vector3 endPos = startPos + dir * moveDistance;

            float elapsed = 0f;
            while (elapsed < moveDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / moveDuration);
                transform.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }

            transform.position = endPos;
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

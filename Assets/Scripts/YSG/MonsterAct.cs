using System.Collections;
using UnityEngine;

public class MonsterAct : Character
{
    [Header("Card Stats")]
    public float maxHealth;
    public float attackPower;
    public float defensePower;

    protected Vector2 moveDelay = new Vector2(0.5f, 2);
    protected float moveDistance = 1;
    protected float moveDuration = 0.5f;
    protected Transform moveTarget;

    private void OnValidate()
    {
        moveDelay.x = Mathf.Max(0.01f, moveDelay.x);
        moveDelay.y = Mathf.Max(moveDelay.x, moveDelay.y);
    }

    private void Awake()
    {
        charData = GetComponent<Card2D>().RuntimeData as CharacterCardData;

        if (charData != null)
        {
            maxHealth = charData.MaxHealth;
            currentHealth = charData.MaxHealth;
            attackPower = charData.AttackPower;
            defensePower = charData.DefensePower;
        }
    }

    private new void Start()
    {
        ChaseTarget();
    }

    protected virtual void Update()
    {
        if (BattleManager.Instance.inBattle)
        {
            if (BattleManager.Instance.battleArea != null)
                moveTarget = BattleManager.Instance.battleArea.transform;
        }
        else
        {
            moveTarget = SetTarget();
        }

        CheckTarget();
    }

    public virtual Transform SetTarget()
    {
        Human[] humans = FindObjectsByType<Human>(FindObjectsSortMode.None);
        moveTarget = null;

        float minDist = float.MaxValue;
        Vector3 myPos = transform.position;

        foreach (var human in humans)
        {
            float dist = Vector3.Distance(myPos, human.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                moveTarget = human.transform;
            }
        }

        return moveTarget;
    }

    public void ChaseTarget() => StartCoroutine(Chase());
    private IEnumerator Chase()
    {
        while (true)
        {
            float waitTime = Random.Range(moveDelay.x, moveDelay.y);
            yield return new WaitForSeconds(waitTime);

            if (moveTarget == null) continue;

            Vector3 startPos = transform.position;
            Vector3 dir = (moveTarget.position - startPos).normalized;
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

    public virtual void CheckTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Human>(out var human))
            {
                StopAllCoroutines();

                if (!BattleManager.Instance.humans.Contains(human))
                {
                    BattleManager.Instance.Unstack(human.GetComponent<Card2D>());
                    BattleManager.Instance.humans.Add(human);
                }

                if (!BattleManager.Instance.monsters.Contains(this))
                    BattleManager.Instance.monsters.Add(this);

                BattleManager.Instance.StartBattle();
                break;
            }
        }
    }

    public override void Die()
    {
        DropItem();

        base.Die();
    }

    public virtual void DropItem()
    {
        MonsterCardData monsterData = charData as MonsterCardData;
        if (monsterData == null || monsterData.Drops == null || monsterData.Drops.Length == 0)
            return;

        Vector3 spawnPos = transform.position + Vector3.up * 0.5f;

        foreach (CardData drop in monsterData.Drops)
        {
            if (drop != null)
            {
                CardManager.Instance.SpawnCard(drop, spawnPos);
                spawnPos += Vector3.right * 0.5f;
            }
        }
    }
}

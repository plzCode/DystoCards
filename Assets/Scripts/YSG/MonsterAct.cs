using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonsterAct : Character
{
    [Header("Card Stats")]
    public float maxHealth;
    public float attackPower;
    public float defensePower;

    protected Vector2 moveDelay = new Vector2(0.5f, 2);
    public float moveSpeed;
    protected Transform moveTarget;

    public TextMeshPro hpValue;

    private void OnValidate()
    {
        moveDelay.x = Mathf.Max(0.01f, moveDelay.x);
        moveDelay.y = Mathf.Max(moveDelay.x, moveDelay.y);
    }

    private void Awake()
    {
        charData = GetComponent<Card2D>().RuntimeData as CharacterCardData;

        if (charData != null && charData is MonsterCardData data)
        {
            maxHealth = data.MaxHealth;
            currentHealth = data.MaxHealth;
            attackPower = data.AttackPower;
            defensePower = data.DefensePower;

            moveSpeed = data.MoveSpeed;
        }
    }

    private new void Start()
    {
        SetCurrentHealth();
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

        if (hpValue != null)
            hpValue.text = currentHealth.ToString("F0");
        else
            SetCurrentHealth();
    }

    private void SetCurrentHealth()
    {
        Transform statAnchor = transform.Find("StatAnchor");
        if (statAnchor != null)
        {
            Transform hp = statAnchor.Find("hp");
            if (hp != null)
            {
                Transform value = hp.Find("Value");
                if (value != null)
                {
                    hpValue = value.GetComponent<TextMeshPro>();
                }
            }
        }
    }

    #region 이동
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

    public void ChaseTarget() => StartCoroutine(MoveCoroutine(true));

    protected IEnumerator MoveCoroutine(bool isChasing)
    {
        while (true)
        {
            float waitTime = Random.Range(moveDelay.x, moveDelay.y);
            yield return new WaitForSeconds(waitTime);

            if (moveTarget == null) continue;

            Vector3 startPos = transform.position;
            Vector3 dir = (moveTarget.position - startPos).normalized * (isChasing ? +1 : -1);
            Vector3 endPos = startPos + dir * moveSpeed;

            float elapsed = 0f;
            while (elapsed < 1)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed);
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
    #endregion

    #region 전투
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

        foreach (var drop in monsterData.Drops)
        {
            if (drop == null || drop.item == null) continue;

            int roll = Random.Range(0, 100);
            if (roll < drop.chance)
            {
                CardManager.Instance.SpawnCard(drop.item, spawnPos);
                spawnPos += Vector3.right * 0.5f;
            }
        }
    }
    #endregion
}

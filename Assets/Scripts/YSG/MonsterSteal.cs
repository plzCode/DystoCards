using UnityEngine;

public class MonsterSteal : MonsterAct
{
    private CardData stealItem;

    protected override void Update()
    {
        if (moveTarget == null)
            moveTarget = SetTarget();

        if (stealItem != null)
            moveTarget = base.SetTarget();

        CheckTarget();

        if (hpValue != null)
            hpValue.text = currentHealth.ToString("F0");
        else
            base.SetCurrentHealth();
    }

    #region 이동
    public override Transform SetTarget()
    {
        Card2D[] items = FindObjectsByType<Card2D>(FindObjectsSortMode.None);
        moveTarget = null;

        float minDist = float.MaxValue;
        Vector3 myPos = transform.position;

        foreach (var item in items)
        {
            var data = item.cardData;
            if (data == null) continue;

            if (data.cardType == CardType.Character ||
                data.cardType == CardType.Facility ||
                data.cardType == CardType.Event) continue;

            float dist = Vector3.Distance(myPos, item.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                moveTarget = item.transform;
            }
        }

        return moveTarget;
    }

    public override void CheckTarget()
    {
        base.CheckTarget();

        if (stealItem != null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Card2D>(out var card) && card.cardData != null)
            {
                var data = card.cardData;
                if (data.cardType == CardType.Character ||
                    data.cardType == CardType.Facility ||
                    data.cardType == CardType.Event) continue;

                stealItem = data;
                CardManager.Instance.DestroyCard(card);

                StopAllCoroutines();
                RunAway();

                break;
            }
        }
    }

    public void RunAway() => StartCoroutine(MoveCoroutine(false));
    #endregion

    #region 전투
    public override void DropItem(Vector3 spawnPos)
    {
        Bounds b = GetWorldBounds();
        b.center = spawnPos;

        while (!MapManager.Instance.AreAllCellsUnlocked(b))
        {
            spawnPos = Vector3.MoveTowards(spawnPos, Vector3.zero, 0.5f);
            b.center = spawnPos;
            if (spawnPos == Vector3.zero) break;
        }

        if (stealItem != null)
        {
            CardManager.Instance.SpawnCard(stealItem, spawnPos);
            spawnPos = Vector3.MoveTowards(spawnPos, Vector3.zero, 0.5f);
        }

        base.DropItem(spawnPos);
    }

    private void OnBecameInvisible()
    {
        var card = GetComponent<Card2D>();
        if (card != null)
            CardManager.Instance.DestroyCard(card);
    }
    #endregion
}

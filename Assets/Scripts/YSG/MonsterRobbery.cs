using System.Collections.Generic;
using UnityEngine;

public class MonsterRobbery : MonsterAct
{
    private List<CardData> stealItems = new List<CardData>();

    protected override void Update()
    {
        if (moveTarget == null)
            moveTarget = SetTarget();

        CheckTarget();

        if (hpValue != null)
            hpValue.text = currentHealth.ToString("F0");
        else
            base.SetCurrentHealth();
    }

    #region 이동
    public override void CheckTarget()
    {
        base.CheckTarget();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Card2D>(out var card) && card.cardData != null)
            {
                var data = card.cardData;
                if (data.cardType == CardType.Character ||
                    data.cardType == CardType.Facility ||
                    data.cardType == CardType.Event) continue;

                stealItems.Add(data);
                CardManager.Instance.DestroyCard(card);
            }
        }
    }
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

        if (stealItems.Count > 0)
        {
            for (int i = 0; i < stealItems.Count; i++)
            {
                CardManager.Instance.SpawnCard(stealItems[i], spawnPos);
                spawnPos = Vector3.MoveTowards(spawnPos, Vector3.zero, 0.5f);
            }
            stealItems.Clear();
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

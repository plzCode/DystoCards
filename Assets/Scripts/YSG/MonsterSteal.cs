using System.Collections;
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
    }

    public override Transform SetTarget()
    {
        Card2D[] items = FindObjectsByType<Card2D>(FindObjectsSortMode.None);
        moveTarget = null;

        float minDist = float.MaxValue;
        Vector3 myPos = transform.position;

        foreach (var item in items)
        {
            if (item.cardData.cardType == CardType.Character) continue;

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

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Card2D>(out var card))
            {
                if (card.cardData.cardType == CardType.Character) continue;

                stealItem = card.cardData;
                CardManager.Instance.DestroyCard(card);

                StopAllCoroutines();
                StartCoroutine(Run());
            }
        }
    }

    private IEnumerator Run()
    {
        while (true)
        {
            float waitTime = Random.Range(moveDelay.x, moveDelay.y);
            yield return new WaitForSeconds(waitTime);

            if (moveTarget == null) continue;

            Vector3 startPos = transform.position;
            Vector3 dir = (moveTarget.position - startPos).normalized;
            Vector3 endPos = startPos - dir * moveDistance;

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

    public override void DropItem()
    {
        base.DropItem();

        CardManager.Instance.SpawnCard(stealItem, transform.position);
    }

    private void OnBecameInvisible()
    {
        var card = GetComponent<Card2D>();
        if (card != null)
            CardManager.Instance.DestroyCard(card);
    }
}

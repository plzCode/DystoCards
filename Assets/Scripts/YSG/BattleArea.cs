using System.Collections.Generic;
using UnityEngine;

public class BattleArea : MonoBehaviour
{
    private Collider2D col;

    private void Start()
    {
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("MapTile"), true);
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (TryAddToBattle<Human>(collision, BattleManager.Instance.humans) ||
            TryAddToBattle<MonsterAct>(collision, BattleManager.Instance.monsters, stopCoroutines: true))
        {
            BattleManager.Instance.Arrange();
            return;
        }

        if (collision.TryGetComponent(out Card2D card))
            PushOutside(card);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Card2D card))
        {
            if (!collision.TryGetComponent(out Human _) &&
                !collision.TryGetComponent(out MonsterAct _))
                PushOutside(card);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Human human))
        {
            if (BattleManager.Instance.humans.Contains(human) && BattleManager.Instance.humans.Count > 1)
            {
                BattleManager.Instance.humans.Remove(human);
                var card = human.GetComponent<Card2D>();
                if (card != null)
                {
                    card.GetComponent<CardTileBarrier>().enabled = true;
                    card.isStackable = true;
                }
            }
        }
    }

    private bool TryAddToBattle<T>(Collider2D collision, List<T> list, bool stopCoroutines = false) where T : Component
    {
        if (collision.TryGetComponent(out T comp) && !list.Contains(comp))
        {
            comp.GetComponent<CardTileBarrier>().enabled = false;

            if (stopCoroutines && comp is MonsterAct monster)
                monster.StopAllCoroutines();

            list.Add(comp);

            BattleManager.Instance.Unstack(comp.GetComponent<Card2D>());

            return true;
        }
        return false;
    }

    private void PushOutside(Card2D card)
    {
        Vector2 center = Vector2.zero;
        Vector2 cardPos = card.transform.position;
        Vector2 direction = (center - cardPos).normalized;

        if (col != null && col.bounds.Contains(center))
        {
            Vector2 areaCenter = col.bounds.center;
            direction = (center - areaCenter).normalized;
        }

        float pushDistance = 0.1f;
        Vector2 newPos = cardPos + direction * pushDistance;
        card.transform.position = newPos;
    }
}

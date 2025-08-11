using System.Collections.Generic;
using UnityEngine;

public class BattleArea : MonoBehaviour
{
    private void Start()
    {
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("MapTile"), true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (TryAddToBattle<Human>(collision, BattleManager.Instance.humans) ||
            TryAddToBattle<TestMonster>(collision, BattleManager.Instance.monsters, stopCoroutines: true))
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
                !collision.TryGetComponent(out TestMonster _))
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
                    card.isStackable = true;
            }
        }
    }

    private bool TryAddToBattle<T>(Collider2D collision, List<T> list, bool stopCoroutines = false) where T : Component
    {
        if (collision.TryGetComponent(out T comp) && !list.Contains(comp))
        {
            if (stopCoroutines && comp is TestMonster monster)
                monster.StopAllCoroutines();

            list.Add(comp);

            BattleManager.Instance.Unstack(comp.GetComponent<Card2D>());

            return true;
        }
        return false;
    }

    private void PushOutside(Card2D card)
    {
        Vector2 center = transform.position;
        Vector2 cardPos = card.transform.position;
        Vector2 direction = (cardPos - center).normalized;
        if (direction == Vector2.zero) direction = Vector2.down;
        float pushDistance = 0.5f;
        Vector2 newPos = cardPos + direction * pushDistance;
        card.transform.position = newPos;
    }
}

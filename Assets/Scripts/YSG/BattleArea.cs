using UnityEngine;

public class BattleArea : MonoBehaviour
{
    private void Start()
    {
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("MapTile"), true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Human human))
        {
            if (!BattleManager.Instance.humans.Contains(human))
            {
                BattleManager.Instance.humans.Add(human);
                human.GetComponent<Card2D>().isStackable = false;
            }
        }
        else if (collision.TryGetComponent(out TestMonster monster))
        {
            if (!BattleManager.Instance.monsters.Contains(monster))
            {
                monster.StopAllCoroutines();
                BattleManager.Instance.monsters.Add(monster);
            }
        }
        else if (collision.TryGetComponent(out Card2D card))
        {
        }

        BattleManager.Instance.Arrange();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Human human))
        {
            if (BattleManager.Instance.humans.Contains(human) && BattleManager.Instance.humans.Count > 1)
            {
                human.GetComponent<Card2D>().isStackable = true;
                BattleManager.Instance.humans.Remove(human);
            }
        }
    }
}

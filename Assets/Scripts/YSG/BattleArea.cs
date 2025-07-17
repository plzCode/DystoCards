using UnityEngine;

public class BattleArea : MonoBehaviour
{
    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);

        if (collision.TryGetComponent(out Human human))
        {
            if (!BattleManager.Instance.humans.Contains(human))
            {
                BattleManager.Instance.humans.Add(human);
            }
        }
        else if (collision.TryGetComponent(out TestMonster monster))
        {
            if (!BattleManager.Instance.monsters.Contains(monster))
            {
                monster.StopAllCoroutines();
                monster.GetComponent<Rigidbody2D>().simulated = false;
                BattleManager.Instance.monsters.Add(monster);
            }
        }
        else return;

        BattleManager.Instance.ArrangeCharacters();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Human human))
        {
            if (BattleManager.Instance.humans.Contains(human) && BattleManager.Instance.humans.Count > 1)
            {
                BattleManager.Instance.humans.Remove(human);
            }
        }
    }
}

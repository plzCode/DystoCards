using UnityEngine;

public class BattleArea : MonoBehaviour
{
    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name);

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
                BattleManager.Instance.monsters.Add(monster);
            }
        }
        else
        {
            Vector3 center = transform.position;
            Vector3 direction = (collision.transform.position - center).normalized;

            float pushDistance = 1; 
            Vector3 newPos = transform.position + direction * (transform.localScale.x * 0.5f + pushDistance);

            collision.transform.position = newPos;
            return;
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

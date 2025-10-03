using System.Collections.Generic;
using UnityEngine;

public class TestDebugger : MonoBehaviour
{
    public void Update()
    {
        //사람 소환
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CardManager.Instance.SpawnCardById("071", new Vector3(0, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            CardManager.Instance.SpawnCardById("072", new Vector3(0, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            CardManager.Instance.SpawnCardById("073", new Vector3(0, 0, 0));
        }

        //몬스터 소환
        if (Input.GetKeyDown(KeyCode.A))
        {
            BattleManager.Instance.SpawnMonster();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            BattleManager.Instance.SpawnMonsterById("801");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            BattleManager.Instance.SpawnMonsterById("802");
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            BattleManager.Instance.SpawnMonsterById("901");
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            BattleManager.Instance.SpawnMonsterById("905");
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            foreach (Transform child in BattleManager.Instance.cards)
                if (child.GetComponent<MonsterAct>() != null)
                    child.GetComponent<Character>()?.Die();
        }


        //목재 소환
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CardManager.Instance.SpawnCardById("001", new Vector3(0, 0, 0));
        }
        //석재 소환
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CardManager.Instance.SpawnCardById("002", new Vector3(0, 0, 0));
        }
        //철재 소환
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CardManager.Instance.SpawnCardById("008", new Vector3(0, 0, 0));
        }

        //도끼 소환
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CardManager.Instance.SpawnCardById("0391", new Vector3(0, 0, 0));
        }
        //나무 상자 소환
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            CardManager.Instance.SpawnCardById("055", new Vector3(0, 0, 0));

        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            CardManager.Instance.SpawnCardById("056", new Vector3(0, 0, 0));
        }
        if(Input.GetKeyDown(KeyCode.Alpha7))
        {
            CardManager.Instance.SpawnCardById("052", new Vector3(0, 0, 0));
        }

        //감자랑 붕대 소환
        if (Input.GetKeyDown(KeyCode.V))
        {
            CardManager.Instance.SpawnCardById("041", new Vector3(0, 0, 0));
            CardManager.Instance.SpawnCardById("021", new Vector3(0, 0, 0));
        }

        //이벤트 실행
        if(Input.GetKeyDown(KeyCode.Z))
        {
            if(UIManager.Instance.eventUIManager != null)
            {
                UIManager.Instance.eventUIManager.OpenEventUI();
            }
        }


        //
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            List<Card2D> charCard = CardManager.Instance.GetCardsByType(CardType.Character);
            List<Card2D> humanCard = CardManager.Instance.GetCharacterType(charCard, CharacterType.Human);

            foreach(var card in humanCard)
            {
                Human human = card.GetComponent<Human>();
                if ((human != null))
                {
                    human.ConsumeFood();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            List<Card2D> charCard = CardManager.Instance.GetCardsByType(CardType.Character);
            List<Card2D> humanCard = CardManager.Instance.GetCharacterType(charCard, CharacterType.Human);

            foreach (var card in humanCard)
            {
                Human human = card.GetComponent<Human>();
                if ((human != null))
                {
                    human.TakeDamage(2f);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            List<Card2D> charCard = CardManager.Instance.GetCardsByType(CardType.Character);
            List<Card2D> humanCard = CardManager.Instance.GetCharacterType(charCard, CharacterType.Human);

            foreach (var card in humanCard)
            {
                Human human = card.GetComponent<Human>();
                if ((human != null))
                {
                    human.Heal(2f);
                }
            }
        }
    }
}

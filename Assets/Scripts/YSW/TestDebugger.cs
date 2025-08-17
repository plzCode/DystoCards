using System.Collections.Generic;
using UnityEngine;

public class TestDebugger : MonoBehaviour
{
    public void Update()
    {
        //사람 소환
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CardManager.Instance.SpawnCardById("071", new Vector3(0, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CardManager.Instance.SpawnCardById("999", new Vector3(0, 0, 0));
        }
        //도끼 소환
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CardManager.Instance.SpawnCardById("0391", new Vector3(0, 0, 0));
        }

        //나무 상자 소환
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CardManager.Instance.SpawnCardById("055", new Vector3(0, 0, 0));

        }
        //목재 소환
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CardManager.Instance.SpawnCardById("001", new Vector3(0, 0, 0));
        }
        //석재 소환
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            CardManager.Instance.SpawnCardById("002", new Vector3(0, 0, 0));
        }
        if(Input.GetKeyDown(KeyCode.Alpha6))
        {
            CardManager.Instance.SpawnCardById("052", new Vector3(0, 0, 0));
        }
        if(Input.GetKeyDown(KeyCode.Alpha7))
        {
            CardManager.Instance.SpawnCardById("056", new Vector3(0, 0, 0));
        }

        //감자랑 붕대 소환
        if (Input.GetKeyDown(KeyCode.V))
        {
            CardManager.Instance.SpawnCardById("041", new Vector3(0, 0, 0));
            CardManager.Instance.SpawnCardById("021", new Vector3(0, 0, 0));
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

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Debug.Log(Recorder.Instance.GetAllStory());
        }
    }
}

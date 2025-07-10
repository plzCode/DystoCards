using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public List<TestCard> playerCards = new List<TestCard>();
    public List<TestCard> monsterCards = new List<TestCard>();

    private bool inBattle = false;

    private void Start()
    {
        TestCard[] allCards = FindObjectsByType<TestCard>(FindObjectsSortMode.None);
        System.Array.Sort(allCards, (a, b) => a.name.CompareTo(b.name));

        foreach (var card in allCards)
        {
            if (card.cardData is CharacterCardData characterCard)
            {
                switch (characterCard.characterType)
                {
                    case CharacterType.Human:
                        playerCards.Add(card);
                        break;

                    case CharacterType.Monster:
                        monsterCards.Add(card);
                        break;
                }
            }
        }
    }

    private void Update()
    {
        playerCards.RemoveAll(card => card == null);
        monsterCards.RemoveAll(card => card == null);

        if (Input.GetKeyUp(KeyCode.T))
        {
            if (!inBattle)
                StartCoroutine(BattleSequence());
        }
    }

    private IEnumerator BattleSequence()
    {
        inBattle = true;

        foreach (var p in playerCards)
        {
            if (monsterCards.Count == 0) break;
            int randIndex = Random.Range(0, monsterCards.Count);
            TestCard target = monsterCards[randIndex];
            yield return StartCoroutine(p.AttackEffect(target));
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);

        foreach (var m in monsterCards)
        {
            if (playerCards.Count == 0) break;
            int randIndex = Random.Range(0, playerCards.Count);
            TestCard target = playerCards[randIndex];
            yield return StartCoroutine(m.AttackEffect(target));
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);

        DebugResult();
        inBattle = false;
    }

    private void DebugResult()
    {
        string playerHealthLog = "플레이어 체력 : ";
        foreach (var p in playerCards)
            playerHealthLog += $"{p.currentHealth} / ";
        playerHealthLog = playerHealthLog.TrimEnd(' ', '/');
        Debug.Log(playerHealthLog);

        string monsterHealthLog = "몬스터 체력 : ";
        foreach (var m in monsterCards)
            monsterHealthLog += $"{m.currentHealth} / ";
        monsterHealthLog = monsterHealthLog.TrimEnd(' ', '/');
        Debug.Log(monsterHealthLog);
    }
}

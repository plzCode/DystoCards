using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public List<Human> humanCards = new List<Human>();
    public List<TestCard> monsterCards = new List<TestCard>();

    private bool inBattle = false;

    private void Start()
    {
        Card2D[] allCards = FindObjectsByType<Card2D>(FindObjectsSortMode.None);
        System.Array.Sort(allCards, (a, b) => a.name.CompareTo(b.name));

        foreach (var card in allCards)
        {
            if (card.cardData is CharacterCardData characterCard)
            {
                switch (characterCard.characterType)
                {
                    case CharacterType.Human:
                        humanCards.Add(card.GetComponent<Human>());
                        break;

                    case CharacterType.Monster:
                        monsterCards.Add(card.GetComponent<TestCard>());
                        break;
                }
            }
        }
    }

    private void Update()
    {
        humanCards.RemoveAll(card => card == null);
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

        foreach (var p in humanCards)
        {
            if (monsterCards.Count == 0) break;
            int randIndex = Random.Range(0, monsterCards.Count);
            var target = monsterCards[randIndex];
            yield return StartCoroutine(AttackEffect(p, target));
            yield return new WaitForSeconds(0.1f);
        }

        foreach (var m in monsterCards)
        {
            if (humanCards.Count == 0) break;
            int randIndex = Random.Range(0, humanCards.Count);
            Human target = humanCards[randIndex];
            yield return StartCoroutine(AttackEffect(m, target));
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);

        DebugResult();
        inBattle = false;
    }

    private IEnumerator AttackEffect(Character attacker, Character target)
    {
        Transform attackerTr = attacker.transform;
        Transform targetTr = target.transform;

        Vector3 originPos = attackerTr.position;
        Vector3 targetPos = targetTr.position;
        Vector3 midPos = Vector3.Lerp(originPos, targetPos, 0.8f);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 10;
            attackerTr.position = Vector3.Lerp(originPos, midPos, t);
            yield return null;
        }

        yield return StartCoroutine(HitEffect(target));

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 10;
            attackerTr.position = Vector3.Lerp(midPos, originPos, t);
            yield return null;
        }

        Debug.Log($"{attacker.name}가 {target.name}를 공격 : 데미지 {attacker.charData.attack_power} / 타겟 체력 {target.currentHealth}");
        target.TakeDamage(attacker.charData.attack_power);
    }

    private IEnumerator HitEffect(Character target)
    {
        SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        Color originalColor = sr.color;

        for (int i = 0; i < 2; i++)
        {
            sr.color = Color.clear;
            yield return new WaitForSeconds(0.1f);
            sr.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void DebugResult()
    {
        string playerHealthLog = "플레이어 체력 : ";
        foreach (var p in humanCards)
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

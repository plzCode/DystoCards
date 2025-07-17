using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [SerializeField] private Transform spawnArea;
    [SerializeField] private List<GameObject> spawnList = new List<GameObject>();
    [Space]
    public List<Human> humans = new List<Human>();
    public List<TestCard> monsters = new List<TestCard>();

    private bool inBattle = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
                        humans.Add(card.GetComponent<Human>());
                        break;

                    case CharacterType.Monster:
                        monsters.Add(card.GetComponent<TestCard>());
                        break;
                }
            }
        }
    }

    private void Update()
    {
        humans.RemoveAll(card => card == null);
        monsters.RemoveAll(card => card == null);

        if (Input.GetKeyUp(KeyCode.R)) // 임시
        {
            for (int i = 0; i < 5; i++)
            {
                SpawnMonster();
            }
        }

        if (Input.GetKeyUp(KeyCode.T)) // 임시
        {
            if (!inBattle)
                StartCoroutine(BattleSequence());
        }
    }

    private void SpawnMonster()
    {
        if (spawnList.Count == 0 || spawnArea == null) return;

        GameObject prefab = spawnList[Random.Range(0, spawnList.Count)];

        Vector3 mapPos = spawnArea.position;
        Vector3 mapScale = spawnArea.localScale;

        float halfWidth = 0.5f * mapScale.x;
        float halfHeight = 0.5f * mapScale.y;

        float randX = Random.Range(mapPos.x - halfWidth, mapPos.x + halfWidth);
        float randY = Random.Range(mapPos.y - halfHeight, mapPos.y + halfHeight);

        Vector3 spawnPos = new Vector3(randX, randY, 0f);

        GameObject go = Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    private IEnumerator BattleSequence()
    {
        inBattle = true;

        foreach (var p in humans)
        {
            if (monsters.Count == 0) break;
            int randIndex = Random.Range(0, monsters.Count);
            var target = monsters[randIndex];
            yield return StartCoroutine(AttackOn(p, target));
            yield return new WaitForSeconds(0.1f);
        }

        foreach (var m in monsters)
        {
            if (humans.Count == 0) break;
            int randIndex = Random.Range(0, humans.Count);
            Human target = humans[randIndex];
            yield return StartCoroutine(AttackOn(m, target));
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);

        DebugResult();
        inBattle = false;
    }

    private IEnumerator AttackOn(Character attacker, Character target)
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

        attacker.Attack(target);
        Debug.Log($"{attacker.name}가 {target.name}를 공격 : 데미지 {attacker.charData.attack_power} / 타겟 체력 {target.currentHealth}");
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
        foreach (var p in humans)
            playerHealthLog += $"{p.currentHealth} / ";
        playerHealthLog = playerHealthLog.TrimEnd(' ', '/');
        Debug.Log(playerHealthLog);

        string monsterHealthLog = "몬스터 체력 : ";
        foreach (var m in monsters)
            monsterHealthLog += $"{m.currentHealth} / ";
        monsterHealthLog = monsterHealthLog.TrimEnd(' ', '/');
        Debug.Log(monsterHealthLog);
    }
}

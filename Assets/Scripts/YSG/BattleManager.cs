using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [SerializeField] private Transform cards;
    public Transform battleArea;
    public Transform spawnArea;
    [SerializeField] private List<GameObject> spawnList = new List<GameObject>();
    [Space]
    public List<Human> humans = new List<Human>();
    public List<TestMonster> monsters = new List<TestMonster>();

    public bool inBattle { get; private set; } = false;

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
        battleArea?.gameObject.SetActive(false);
    }

    private void Update()
    {
        humans.RemoveAll(card => card == null);
        monsters.RemoveAll(card => card == null);

        if (Input.GetKeyUp(KeyCode.T)) // 임시
        {
            SpawnMonster();
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
        go.transform.SetParent(cards);
    }

    public void StartBattle()
    {
        if (!inBattle) ArrangeCharacters();
        StartCoroutine(BattleSequence());
    }

    public void ArrangeCharacters()
    {
        if (humans.Count == 0 && monsters.Count == 0) return;

        float spacing = 1.5f;
        float margin = 1.0f;

        float humanWidth = (humans.Count - 1) * spacing;
        float monsterWidth = (monsters.Count - 1) * spacing;
        float maxWidth = Mathf.Max(humanWidth, monsterWidth) + margin * 2f;
        float totalHeight = 2f + margin * 2f;

        if (!battleArea.gameObject.activeSelf)
        {
            Vector3 totalPos = Vector3.zero;
            int totalCount = 0;

            foreach (var h in humans)
            {
                totalPos += h.transform.position;
                totalCount++;
            }
            foreach (var m in monsters)
            {
                totalPos += m.transform.position;
                totalCount++;
            }

            if (totalCount > 0)
                battleArea.position = totalPos / totalCount;
        }

        battleArea.localScale = new Vector3(maxWidth, totalHeight, 1f);
        battleArea.gameObject.SetActive(true);

        float topY = totalHeight / 2f - margin;
        float bottomY = -totalHeight / 2f + margin;

        float humanStartX = -humanWidth / 2f;
        for (int i = 0; i < humans.Count; i++)
        {
            Vector3 localPos = new Vector3(humanStartX + i * spacing, topY, 0f);
            humans[i].transform.position = battleArea.position + localPos;
        }

        float monsterStartX = -monsterWidth / 2f;
        for (int i = 0; i < monsters.Count; i++)
        {
            Vector3 localPos = new Vector3(monsterStartX + i * spacing, bottomY, 0f);
            monsters[i].transform.position = battleArea.position + localPos;
        }
    }

    private IEnumerator BattleSequence()
    {
        inBattle = true;

        while (humans.Count > 0 && monsters.Count > 0)
        {
            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < humans.Count; i++)
            {
                if (monsters.Count == 0) break;

                Human p = humans[i];
                int randIndex = Random.Range(0, monsters.Count);
                var target = monsters[randIndex];

                yield return StartCoroutine(AttackEffect(p, target));
                yield return new WaitForSeconds(0.5f);

                if (monsters.Count == 0) break;
            }

            for (int i = 0; i < monsters.Count; i++)
            {
                if (humans.Count == 0) break;

                TestMonster m = monsters[i];
                int randIndex = Random.Range(0, humans.Count);
                var target = humans[randIndex];

                yield return StartCoroutine(AttackEffect(m, target));
                yield return new WaitForSeconds(0.5f);
            }

            DebugResult();
        }

        inBattle = false;

        foreach (var monster in monsters)
        {
            monster.GetComponent<Rigidbody2D>().simulated = true;
            monster?.StartMove();
        }

        humans.Clear();
        monsters.Clear();
        battleArea?.gameObject.SetActive(false);
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

        attacker.Attack(target);
        Debug.Log($"{attacker.name}가 {target.name}를 공격 : 타겟 {target.currentHealth}-{attacker.charData.attack_power}={target.currentHealth - attacker.charData.attack_power}");

        ArrangeCharacters();
    }

    private IEnumerator HitEffect(Character hitter)
    {
        SpriteRenderer sr = hitter.GetComponent<SpriteRenderer>();
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

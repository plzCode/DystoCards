using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnEntry
{
    public GameObject prefab;
    [Range(0, 100)] public float spawnProbability;
}

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    public Transform cards;
    public Transform battleArea;
    public Transform spawnArea;
    [SerializeField] private List<SpawnEntry> spawnList = new List<SpawnEntry>();

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

    #region 소환
    private void SpawnMonster()
    {
        if (spawnList.Count == 0 || spawnArea == null) return;

        Vector3 mapPos = spawnArea.position;
        Vector3 mapScale = spawnArea.localScale;

        float halfWidth = 0.5f * mapScale.x;
        float halfHeight = 0.5f * mapScale.y;

        foreach (var entry in spawnList)
        {
            if (Random.Range(0, 100) <= entry.spawnProbability)
            {
                float randX = Random.Range(mapPos.x - halfWidth, mapPos.x + halfWidth);
                float randY = Random.Range(mapPos.y - halfHeight, mapPos.y + halfHeight);

                Vector3 spawnPos = new Vector3(randX, randY, 0);
                GameObject go = Instantiate(entry.prefab, spawnPos, Quaternion.identity);
                go.transform.SetParent(cards);
            }
        }
    }

    public void TrySpawnMonster(int probability = 0)
    {
        if (spawnList.Count == 0 || spawnArea == null) return;

        if (Random.value < (float)(probability / 100))
        {
            SpawnMonster();
        }
    }
    #endregion

    #region 전투
    public void StartBattle()
    {
        if (!inBattle) ArrangeCharacters();
        StartCoroutine(BattleSequence());
    }

    public void ArrangeCharacters()
    {
        if (humans.Count == 0 && monsters.Count == 0) return;

        float margin = 1f;
        float spacingX = 0.5f;
        float spacingY = 1f;

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

        List<float> humanWidths = new List<float>();
        List<float> humanHeights = new List<float>();
        foreach (var h in humans)
        {
            var sr = h.GetComponent<SpriteRenderer>();
            float w = sr?.bounds.size.x ?? 1f;
            float hgt = sr?.bounds.size.y ?? 1f;
            humanWidths.Add(w);
            humanHeights.Add(hgt);
        }

        List<float> monsterWidths = new List<float>();
        List<float> monsterHeights = new List<float>();
        foreach (var m in monsters)
        {
            var sr = m.GetComponent<SpriteRenderer>();
            float w = sr?.bounds.size.x ?? 1f;
            float hgt = sr?.bounds.size.y ?? 1f;
            monsterWidths.Add(w);
            monsterHeights.Add(hgt);
        }

        float humanTotalWidth = -spacingX;
        foreach (float w in humanWidths)
            humanTotalWidth += w + spacingX;

        float monsterTotalWidth = -spacingX;
        foreach (float w in monsterWidths)
            monsterTotalWidth += w + spacingX;

        float maxWidth = Mathf.Max(humanTotalWidth, monsterTotalWidth) + margin * 2;
        float maxHumanHeight = humanHeights.Count > 0 ? Mathf.Max(humanHeights.ToArray()) : 1f;
        float maxMonsterHeight = monsterHeights.Count > 0 ? Mathf.Max(monsterHeights.ToArray()) : 1f;
        float totalHeight = maxHumanHeight + maxMonsterHeight + spacingY + margin * 2;

        battleArea.localScale = new Vector3(maxWidth, totalHeight, 1);
        battleArea.gameObject.SetActive(true);

        float centerY = battleArea.position.y;
        float humanY = centerY + (maxHumanHeight + spacingY) / 2;
        float monsterY = centerY - (maxMonsterHeight + spacingY) / 2;

        float humanX = battleArea.position.x - humanTotalWidth / 2;
        for (int i = 0; i < humans.Count; i++)
        {
            float width = humanWidths[i];
            humans[i].transform.position = new Vector3(humanX + width / 2, humanY, 0);
            humanX += width + spacingX;
        }

        float monsterX = battleArea.position.x - monsterTotalWidth / 2;
        for (int i = 0; i < monsters.Count; i++)
        {
            float width = monsterWidths[i];
            monsters[i].transform.position = new Vector3(monsterX + width / 2, monsterY, 0);
            monsterX += width + spacingX;
        }
    }

    private IEnumerator BattleSequence()
    {
        inBattle = true;

        while (humans.Count > 0 && monsters.Count > 0)
        {
            List<Character> turnOrder = new List<Character>();
            foreach (var h in humans) if (h != null) turnOrder.Add(h);
            foreach (var m in monsters) if (m != null) turnOrder.Add(m);

            Shuffle(turnOrder);

            foreach (var attacker in turnOrder)
            {
                if (humans.Count == 0 || monsters.Count == 0) break;
                if (attacker == null) continue;

                Character target = null;

                if (attacker is Human)
                {
                    if (monsters.Count == 0) break;
                    target = monsters[Random.Range(0, monsters.Count)];
                }
                else if (attacker is TestMonster)
                {
                    if (humans.Count == 0) break;
                    target = humans[Random.Range(0, humans.Count)];
                }

                if (target == null) continue;

                yield return StartCoroutine(AttackEffect(attacker, target));
                yield return new WaitForSeconds(0.1f);
            }

            DebugResult();
        }

        inBattle = false;

        foreach (var monster in monsters)
        {
            if (monster != null)
            {
                monster.GetComponent<Rigidbody2D>().simulated = true;
                monster.StartMove();
            }
        }

        humans.Clear();
        monsters.Clear();
        battleArea?.gameObject.SetActive(false);
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randIndex];
            list[randIndex] = temp;
        }
    }
    #endregion

    #region 효과
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
        Debug.Log($"{attacker.name}가 {target.name}를 공격 : 타겟 {target.currentHealth}-{attacker.charData.AttackPower}={target.currentHealth - attacker.charData.AttackPower}");

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

        sr.color = originalColor;
    }
    #endregion

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

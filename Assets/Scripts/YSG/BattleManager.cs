using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class SpawnEntry
{
    public CardData cardData;
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
        cards = CardManager.Instance.cardParent;
        battleArea?.gameObject.SetActive(false);
    }

    private void Update()
    {
        humans.RemoveAll(h => h == null);
        monsters.RemoveAll(m => m == null);

        if (Input.GetKeyUp(KeyCode.T))
            SpawnMonster();
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
            if (entry.cardData == null) continue;
            if (Random.value <= entry.spawnProbability / 100)
            {
                float randX = Random.Range(mapPos.x - halfWidth, mapPos.x + halfWidth);
                float randY = Random.Range(mapPos.y - halfHeight, mapPos.y + halfHeight);
                Vector3 spawnPos = new Vector3(randX, randY, 0);
                var card = CardManager.Instance.SpawnCard(entry.cardData, spawnPos);
                if (card != null)
                {
                    card.GetComponent<Card2D>().isStackable = false;
                    card.AddComponent<TestMonster>();
                    card.transform.SetParent(cards);
                }
            }
        }
    }

    public void TrySpawnMonster(int probability = 0)
    {
        if (spawnList.Count == 0 || spawnArea == null) return;
        if (Random.value < (float)probability / 100f)
            SpawnMonster();
    }
    #endregion

    #region 전투
    public void StartBattle()
    {
        if (inBattle) return;
        UnstackAll();
        Arrange();
        StartCoroutine(BattleSequence());
    }

    public void Arrange()
    {
        if (humans.Count == 0 && monsters.Count == 0) return;

        float margin = 0.05f;
        float spacingX = 0.1f;
        float spacingY = 0.2f;

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

                if (attacker is Human human && !humans.Contains(human)) continue;
                if (attacker is TestMonster monster && !monsters.Contains(monster)) continue;

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

                yield return StartCoroutine(AttackCoroutine(attacker, target));
                yield return new WaitForSeconds(0.1f);

                Arrange();
            }
        }

        inBattle = false;

        EndBattle(humans);
        EndBattle(monsters);

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

    private void EndBattle<T>(List<T> characters) where T : Character
    {
        foreach (var c in characters)
        {
            if (c == null) continue;

            var sr = c.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = Color.white;

            if (c is TestMonster monster)
            {
                monster.ChaseTarget();
            }

            var card = c.GetComponent<Card2D>();
            if (card != null) card.isStackable = true;
        }
    }
    #endregion

    #region 카드
    private void UnstackAll()
    {
        foreach (var human in humans)
        {
            var card = human.GetComponent<Card2D>();
            if (card != null) Unstack(card);
        }
        foreach (var monster in monsters)
        {
            var card = monster.GetComponent<Card2D>();
            if (card != null) Unstack(card);
        }
    }

    public void Unstack(Card2D card)
    {
        if (card.parentCard != null)
        {
            card.parentCard.childCards.Remove(card);
            card.parentCard = null;
        }
        foreach (var child in new List<Card2D>(card.childCards))
        {
            if (child != null)
                Unstack(child);
        }
        card.childCards.Clear();
        card.transform.SetParent(cards);
    }

    private IEnumerator AttackCoroutine(Character attacker, Character target)
    {
        if (attacker == null || target == null) yield break;

        Transform attackerTr = attacker.transform;
        Transform targetTr = target.transform;

        if (attackerTr == null || targetTr == null) yield break;

        Vector3 originPos = attackerTr.position;
        Vector3 targetPos = targetTr.position;
        Vector3 midPos = Vector3.Lerp(originPos, targetPos, 0.8f);

        float t = 0;
        while (t < 1)
        {
            if (attacker == null || target == null) yield break;
            if (attackerTr == null) yield break;

            t += Time.deltaTime * 10;
            attackerTr.position = Vector3.Lerp(originPos, midPos, t);
            yield return null;
        }

        if (attacker != null && target != null)
        {
            attacker.Attack(target);
        }

        yield return StartCoroutine(HitCoroutine(target));

        t = 0;
        while (t < 1)
        {
            if (attacker == null || target == null) yield break;
            if (attackerTr == null) yield break;

            t += Time.deltaTime * 10;
            attackerTr.position = Vector3.Lerp(midPos, originPos, t);
            yield return null;
        }
    }

    private IEnumerator HitCoroutine(Character hitter)
    {
        if (hitter == null) yield break;

        SpriteRenderer sr = hitter.GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        for (int i = 0; i < 2; i++)
        {
            if (sr == null) yield break;
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            if (sr == null) yield break;
            sr.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }

        if (sr != null)
            sr.color = Color.white;
    }
    #endregion
}

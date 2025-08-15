using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    public Transform cards;
    public Transform battleArea;
    public Transform spawnPoint;
    [Space]
#if UNITY_EDITOR
    [SerializeField] private string spawnId;
    [SerializeField] private int spawnCount = 1;
#endif
    [SerializeField] private List<MonsterCardData> monsterCardList = new List<MonsterCardData>();

    [Space]
    public List<Human> humans = new List<Human>();
    public List<MonsterAct> monsters = new List<MonsterAct>();

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


#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.Alpha1))
            CardManager.Instance.SpawnCardByName("성기훈", Vector3.zero);

        if (Input.GetKeyUp(KeyCode.T)) // 몬스터 소환 테스트 (임시)
            SpawnMonster();

        if (Input.GetKeyUp(KeyCode.Y)) // 몬스터 단일 소환 테스트 (임시)
            SpawnMonsterById(spawnId, spawnCount);

        if (Input.GetKeyUp(KeyCode.D)) // 아이템 드랍 테스트 (임시)
        {
            foreach (Transform child in cards)
            {
                if (child.GetComponent<Human>() != null)
                    child.GetComponent<Character>()?.Die();
                if (child.GetComponent<MonsterAct>() != null)
                    child.GetComponent<Character>()?.Die();
            }
        }

        if (Input.GetKeyUp(KeyCode.Delete)) // 모든 카드 제거 테스트 (임시)
        {
            foreach (Transform child in cards)
                CardManager.Instance.DestroyCard(child.GetComponent<Card2D>());
        }
#endif
    }

    #region 소환
    public void SpawnMonster()
    {
        if (monsterCardList.Count == 0 || spawnPoint == null) return;

        Vector3 mapPos = spawnPoint.position;
        Vector3 mapScale = spawnPoint.localScale;
        float halfWidth = 0.5f * mapScale.x;
        float halfHeight = 0.5f * mapScale.y;

        List<MonsterCardData> candidates = new List<MonsterCardData>();
        foreach (var mon in monsterCardList)
        {
            if (TurnManager.Instance.TurnCount >= mon.SpawnTurn)
                candidates.Add(mon);
        }

        if (candidates.Count == 0) return;

        int spawnedCount = 0;
        MonsterCardData toSpawn = candidates[Random.Range(0, candidates.Count)];
        spawnedCount += SpawrMonsterOne(toSpawn, mapPos, halfWidth, halfHeight);

        while (spawnedCount < 3 && toSpawn.SpawnChance > 0 && Random.value <= toSpawn.SpawnChance / 100f)
        {
            spawnedCount += SpawrMonsterOne(toSpawn, mapPos, halfWidth, halfHeight);
        }
    }

    public void SpawnMonsterById(string cardId, int count)
    {
        MonsterCardData mon = monsterCardList.Find(m => m.cardId == cardId);
        if (mon == null || spawnPoint == null) return;

        Vector3 mapPos = spawnPoint.position;
        Vector3 mapScale = spawnPoint.localScale;
        float halfWidth = 0.5f * mapScale.x;
        float halfHeight = 0.5f * mapScale.y;

        for (int i = 0; i < count; i++)
        {
            SpawrMonsterOne(mon, mapPos, halfWidth, halfHeight);
        }
    }

    private int SpawrMonsterOne(MonsterCardData mon, Vector3 mapPos, float halfWidth, float halfHeight)
    {
        var card = CardManager.Instance.SpawnCard(mon, spawnPoint.position);
        if (card == null) return 0;

        card.GetComponent<Card2D>().isStackable = false;

        switch (mon.Act)
        {
            case MonsterActionType.Default:
                card.AddComponent<MonsterAct>();
                break;
            case MonsterActionType.Steal:
                card.AddComponent<MonsterSteal>();
                break;
            case MonsterActionType.Robbery:
                card.AddComponent<MonsterAct>();
                break;
        }

        card.transform.SetParent(cards);
        return 1;
    }
    #endregion

    #region 전투
    public void StartBattle()
    {
        if (inBattle) return;
        Arrange();
        StartCoroutine(BattleSequence());
    }

    public void Arrange()
    {
        UnstackAll();

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
                h.transform.SetParent(cards);
                totalPos += h.transform.position;
                totalCount++;
            }
            foreach (var m in monsters)
            {
                m.transform.SetParent(cards);
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
            humanWidths.Add(sr?.bounds.size.x ?? 1f);
            humanHeights.Add(sr?.bounds.size.y ?? 1f);
        }

        List<float> monsterWidths = new List<float>();
        List<float> monsterHeights = new List<float>();
        foreach (var m in monsters)
        {
            var sr = m.GetComponent<SpriteRenderer>();
            monsterWidths.Add(sr?.bounds.size.x ?? 1f);
            monsterHeights.Add(sr?.bounds.size.y ?? 1f);
        }

        float humanTotalWidth = -spacingX;
        foreach (float w in humanWidths) humanTotalWidth += w + spacingX;

        float monsterTotalWidth = -spacingX;
        foreach (float w in monsterWidths) monsterTotalWidth += w + spacingX;

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
                if (attacker is MonsterAct monster && !monsters.Contains(monster)) continue;

                Character target = null;

                if (attacker is Human)
                {
                    if (monsters.Count == 0) break;
                    target = monsters[Random.Range(0, monsters.Count)];
                }
                else if (attacker is MonsterAct)
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

    public void EndBattle<T>(List<T> characters) where T : Character
    {
        foreach (var c in characters)
        {
            if (c == null) continue;

            var sr = c.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = Color.white;

            if (c.charData.characterType == CharacterType.Human)
            {
                var humanCard = c.GetComponent<Card2D>();
                humanCard.isStackable = true;
            }
            else if (c.charData.characterType == CharacterType.Monster)
            {
                var monsterCard = c.GetComponent<Card2D>();
                var monsterData = monsterCard.cardData as MonsterCardData;
                if (monsterData == null) continue;

                switch (monsterData.Act)
                {
                    case MonsterActionType.Default:
                        c.GetComponent<MonsterAct>().ChaseTarget();
                        break;
                    case MonsterActionType.Steal:
                        c.GetComponent<MonsterSteal>().RunAway();
                        break;
                }
            }
        }
    }
    #endregion

    #region 카드
    public void UnstackAll()
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
        if (card.cardData.cardType == CardType.Character) card.isStackable = false;
        card.transform.SetParent(cards);

        if (card.parentCard != null)
        {
            card.parentCard.childCards.Remove(card);
            card.parentCard = null;
        }
        foreach (var child in new List<Card2D>(card.childCards))
        {
            if (child != null)
            {
                Unstack(child);
            }
        }
        foreach (Transform childTrans in card.transform)
        {
            Card2D childCard = childTrans.GetComponent<Card2D>();
            if (childCard != null)
            {
                Unstack(childCard);
            }
        }

        card.childCards.Clear();
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

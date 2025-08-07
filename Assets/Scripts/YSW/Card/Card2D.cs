using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class Card2D : MonoBehaviour
{
    [SerializeField] public CardData cardData;
    
    [SerializeField] private CardData runtimeData;

    public CardData RuntimeData => runtimeData;

    public static int globalSortingOrder = 0;
    public LayerMask cardLayer = 1<<6;

    private Vector3 dragOffset;
    private bool isDragging = false;

    public Card2D parentCard;
    public List<Card2D> childCards = new List<Card2D>();

    private Transform dragGroupRoot;
    private Dictionary<Card2D, Transform> originalParents = new Dictionary<Card2D, Transform>();

    private CardUIRenderer uiRenderer;

    public MMF_Func cardAnim;

    public bool isInitialized = false;
    public bool isStackable = true;
    public Dissolve dissolveEffect;

    private void Awake()
    {
        uiRenderer = GetComponent<CardUIRenderer>();
        cardAnim = GetComponentInChildren<MMF_Func>();
        dissolveEffect = GetComponentInChildren<Dissolve>();
    }
    private void Start()
    {
        /*if (cardData != null)
        {
            runtimeData = cardData.Clone();

            // UI 이벤트 바인딩
            if (runtimeData is CharacterCardData characterData)
            {
                characterData.OnDataChanged += OnCardDataChanged;
            }            
        }
        RenderCardUI();*/
    }
    private void OnDestroy()
    {
        if (runtimeData is CharacterCardData characterData)
        {
            characterData.OnDataChanged -= OnCardDataChanged;
        }
    }

    #region Mouse Code
    public void StartDragging(Vector3 mouseWorld)
    {        
        dragOffset = transform.position - new Vector3(mouseWorld.x, mouseWorld.y, 0);

        dragGroupRoot = new GameObject("DragGroup").transform;
        dragGroupRoot.position = transform.position;

        // 스택 해제: 논리 + 계층 구조 모두 제거
        if (parentCard != null)
        {
            parentCard.childCards.Remove(this);
            parentCard = null;
            transform.SetParent(CardManager.Instance.cardParent); // Hierarchy 창에서 연결 제거
        }

        CollectStackBelow(this, dragGroupRoot);
        BringToFrontRecursive(this);
        isDragging = true;
    }

    public void Dragging(Vector3 mouseWorld)
    {
        if (dragGroupRoot != null)
        {   
            dragGroupRoot.position = new Vector3(mouseWorld.x, mouseWorld.y, 0) + dragOffset;
        }
    }

    public void EndDragging()
    {
        isDragging = false;

        RestoreParents();

        dragGroupRoot.DetachChildren();
        Destroy(dragGroupRoot.gameObject);

        Card2D target = GetFirstOverlappingCard();
        if (target != null && !IsInHierarchy(this, target))
        {
            // 가장 아래쪽 자식까지 내려가서 등록
            Card2D actualTarget = GetDeepestChild(target);
            StackOnto(actualTarget);
        }

        BringToFrontRecursive(this);
    }
    #endregion

    public void OnCardDataChanged()
    {
        RenderCardUI(); // 카드 UI 다시 그림
    }

    public virtual void OnUse()
    {

    }
    public virtual void OnTurnEffect()
    {

    }

    protected Card2D GetDeepestChild(Card2D card)
    {
        if (card.childCards.Count == 0)
            return card;

        // 가장 마지막 자식 기준으로 깊게 탐색
        return GetDeepestChild(card.childCards[card.childCards.Count - 1]);
    }

    // 자기 자신 또는 자식 중에 target이 있는지 확인 → 순환 참조 방지
    protected bool IsInHierarchy(Card2D root, Card2D target)
    {
        if (root == target)
            return true;

        foreach (var child in root.childCards)
        {
            if (IsInHierarchy(child, target))
                return true;
        }

        return false;
    }

    // ==============================
    // 스택 처리
    // ==============================


    public virtual void StackOnto(Card2D target) //현재 카드를 target 카드 위에 스택(부착)하는 역할.
    {
        if(target.isStackable == false)
        {
            Debug.LogWarning($"Cannot stack onto {target.name} because it is not stackable.");
            return;
        }

        // 기존 부모 카드가 있다면, 자식 리스트에서 자신을 제거
        if (parentCard != null)
        {
            parentCard.childCards.Remove(this);
        }

        // 새 부모 카드 설정
        parentCard = target;

        // 중복 추가 방지
        if (!target.childCards.Contains(this))
        {
            target.childCards.Add(this);
        }

        // 위치 및 계층 구조 조정
        transform.SetParent(target.transform);
        transform.position = target.transform.position + new Vector3(0.0f, -0.5f, -0.01f);

        BringToFrontRecursive(this);
    }

    protected void CollectStackBelow(Card2D card, Transform groupRoot) //특정 카드와 그 자식들 모두를 한 그룹(groupRoot) 아래로 묶음.
    {
        var allCards = GetStackFrom(card);

        originalParents.Clear();
        foreach (var c in allCards)
        {
            originalParents[c] = c.transform.parent;
            c.transform.SetParent(groupRoot);
        }
    }

    protected void RestoreParents() //CollectStackBelow로 그룹화했던 카드들의 부모 관계를 원래대로 복원.
    {
        foreach (var kv in originalParents)
        {
            kv.Key.transform.SetParent(kv.Value);
        }

        originalParents.Clear();
    }

    protected Card2D GetFirstOverlappingCard() //현재 카드와 겹쳐진 카드들 중에서 첫 번째 적합한 카드(자기 자신이나 자식이 아닌)를 찾음.
    {
        Collider2D[] results = new Collider2D[10];
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(cardLayer);
        filter.useTriggers = true;

        int count = GetComponent<BoxCollider2D>().Overlap(filter, results);
        for (int i = 0; i < count; i++)
        {
            Card2D other = results[i].GetComponent<Card2D>();

            // 자기 자신이거나 자신의 자식이면 무시
            if (other != null && other != this && !IsInHierarchy(this, other))
            {
                return other;
            }
        }

        return null;
    }

    public void BringToFrontRecursive(Card2D card) //카드 스택 전체의 스프라이트 렌더링 순서를 갱신하여 앞쪽으로 보이도록 함.
    {
        List<Card2D> stack = GetStackFrom(card);
        /*foreach (var c in stack)
        {
            SpriteRenderer sr = c.GetComponent<SpriteRenderer>();
            sr.sortingOrder = globalSortingOrder++;
        }*/
        foreach (var c in stack)
        {
            int baseOrder = globalSortingOrder += 3;

            // 카드 본체 렌더러
            SpriteRenderer sr = c.GetComponent<SpriteRenderer>();
            sr.sortingOrder = baseOrder;

            // 자식들의 SpriteRenderer 순서도 조정
            var childRenderers = c.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
            foreach (var childRenderer in childRenderers)
            {
                if (childRenderer != sr) // 본체는 이미 처리했으므로 제외
                {
                    childRenderer.sortingOrder = baseOrder + 1;
                }
            }

            var tmp = c.GetComponentsInChildren<TextMeshPro>(includeInactive: true);
            foreach (var t in tmp)
            {
                t.sortingOrder = baseOrder + 2; // 아이콘보다 더 앞으로
            }
        }
    }

    protected List<Card2D> GetStackFrom(Card2D root) //특정 카드(root)부터 그 자식들을 모두 포함하는 리스트 반환.
    {
        List<Card2D> result = new List<Card2D>();
        CollectChildrenRecursive(root, result);
        return result;
    }

    protected void CollectChildrenRecursive(Card2D card, List<Card2D> result) //재귀적으로 카드와 자식들을 리스트에 추가.
    {
        result.Add(card);
        foreach (var child in card.childCards)
        {
            CollectChildrenRecursive(child, result);
        }
    }

    public void DetachChildrenBeforeDestroy() //카드가 파괴되기 전에 자식 카드들을 계층 구조에서 분리하고 논리적으로도 분리함.
    {
        // 부모와의 연결 먼저 해제
        if (parentCard != null)
        {
            parentCard.childCards.Remove(this);
            parentCard = null;
        }

        // 자식 카드들 분리
        for (int i = childCards.Count - 1; i >= 0; i--)
        {
            var child = childCards[i];
            if (child == null) continue;

            child.parentCard = null;
            child.transform.SetParent(CardManager.Instance.cardParent);
            child.transform.position += new Vector3(0.5f, 0.5f, 0f);
        }

        childCards.Clear();
    }

    public bool IsCardType(CardData data, CardType type)
    {
        return data != null && data.cardType == type;
    }
    public bool IsCharacterOfType(CardData data, CharacterType characterType)
    {
        return data is CharacterCardData charData &&
               data.cardType == CardType.Character &&
               charData.characterType == characterType;
    }

    //cardUI 렌더링
    public void RenderCardUI()
    {
        if (uiRenderer == null || runtimeData == null) return;

        var stats = GetStatDictionaryFromCardData(runtimeData);
        uiRenderer.RenderStats(stats);
        uiRenderer.RenderName(runtimeData.cardName);
        uiRenderer.RenderImage(runtimeData.cardImage);
        BringToFrontRecursive(this);
    }

    public Dictionary<string, float> GetStatDictionaryFromCardData(CardData data)
    {
        Dictionary<string, float> stats = new();

        switch (data)
        {
            case FoodCardData food:
                stats["hungerRecovery"] = food.hungerRestore;
                break;
            case EquipmentCardData equip:
                stats["attack"] = equip.attackPower;
                stats["defense"] = equip.defensePower;
                break;
            case HealCardData heal:
                stats["hp"] = heal.healthAmount;
                stats["sanity"] = heal.mentalAmount;
                stats["stamina"] = heal.staninaAmount;
                break;
            case HumanCardData human:                
                stats["hp"] = human.CurrentHealth;
                stats["sanity"] = human.CurrentMentalHealth;
                stats["hunger"] = human.CurrentHunger;
                stats["stamina"] = human.Stamina;
                stats["attack"] = human.AttackPower;
                stats["defense"] = human.DefensePower;
                stats["consumeHunger"] = human.ConsumeHunger;
                break;
            case CharacterCardData ch:
                stats["hp"] = ch.MaxHealth;
                stats["attack"] = ch.AttackPower;
                stats["defense"] = ch.DefensePower;
                break;
        }

        stats["size"] = data.size;
        return stats;
    }

    public T GetRuntimeData<T>() where T : CardData
    {
        return runtimeData as T;
    }
    public void SetRuntimeData(CardData data)
    {
        runtimeData = data;
        
        // 이벤트 연결 등도 여기서 처리
        if (RuntimeData is CharacterCardData stats)
        {
            stats.OnDataChanged += OnCardDataChanged;
        }

        // 명시적으로 초기화 메서드 호출
        InitializeCard();
    }
    private void InitializeCard()
    {
        if (isInitialized) return;

        RenderCardUI();  // 이름, 이미지, 스탯 등 렌더링

        isInitialized = true;
    }

    public IEnumerator MoveItemLerp(Transform item, Vector3 target, float duration)
    {
        Vector3 start = item.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration); // 부드러운 curve 적용
            item.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        item.position = target;
    }

}

    // 이전에 쓰던 코드
    /*private void OnMouseDown()
    {
        //Debug.Log($"Dragging card: {transform.name}");
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragOffset = transform.position - new Vector3(mouseWorld.x, mouseWorld.y, 0);

        dragGroupRoot = new GameObject("DragGroup").transform;
        dragGroupRoot.position = transform.position;

        // 스택 해제: 논리 + 계층 구조 모두 제거
        if (parentCard != null)
        {
            parentCard.childCards.Remove(this);
            parentCard = null;
            transform.SetParent(CardManager.Instance.cardParent); // Hierarchy 창에서 연결 제거
        }

        CollectStackBelow(this, dragGroupRoot);
        BringToFrontRecursive(this);
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (dragGroupRoot != null)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dragGroupRoot.position = new Vector3(mouseWorld.x, mouseWorld.y, 0) + dragOffset;
        }
    }
    private void OnMouseUp()
    {
        isDragging = false;

        RestoreParents();

        dragGroupRoot.DetachChildren();
        Destroy(dragGroupRoot.gameObject);

        Card2D target = GetFirstOverlappingCard();
        if (target != null && !IsInHierarchy(this, target))
        {
            // 가장 아래쪽 자식까지 내려가서 등록
            Card2D actualTarget = GetDeepestChild(target);
            StackOnto(actualTarget);
        }

        BringToFrontRecursive(this);
    }*/
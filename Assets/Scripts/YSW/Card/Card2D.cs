using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class Card2D : MonoBehaviour
{
    [SerializeField] public CardData cardData;

    public static int globalSortingOrder = 0;
    public LayerMask cardLayer;

    private Vector3 dragOffset;
    private bool isDragging = false;

    public Card2D parentCard;
    public List<Card2D> childCards = new List<Card2D>();

    private Transform dragGroupRoot;
    private Dictionary<Card2D, Transform> originalParents = new Dictionary<Card2D, Transform>();

    private void OnMouseDown()
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
            transform.SetParent(null); // Hierarchy 창에서 연결 제거
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

    protected void BringToFrontRecursive(Card2D card) //카드 스택 전체의 스프라이트 렌더링 순서를 갱신하여 앞쪽으로 보이도록 함.
    {
        List<Card2D> stack = GetStackFrom(card);
        foreach (var c in stack)
        {
            SpriteRenderer sr = c.GetComponent<SpriteRenderer>();
            sr.sortingOrder = globalSortingOrder++;
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

    protected void DetachChildrenBeforeDestroy() //카드가 파괴되기 전에 자식 카드들을 계층 구조에서 분리하고 논리적으로도 분리함.
    {
        foreach (var child in childCards)
        {
            if (child == null) continue;

            // 계층적으로 분리
            child.transform.SetParent(null);

            // 논리적으로도 분리
            child.parentCard = null;

            // 위치도 약간 이동시켜 보기 좋게
            child.transform.position += new Vector3(0.5f, 0.5f, 0f);
        }

        childCards.Clear();
    }

}

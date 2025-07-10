using System.Collections.Generic;
using UnityEngine;

public class CombinationManager : MonoBehaviour
{
    [SerializeField] private List<RecipeCardData> recipes; // 조합 가능한 레시피 목록
    [SerializeField] private GameObject cardPrefab; // 결과 카드 프리팹

    private void Update()
    {
        Card2D[] allCards = FindObjectsByType<Card2D>(FindObjectsSortMode.None);
        List<Card2D> topCards = new List<Card2D>();

        // 현재 존재하는 모든 카드 중에서 최상위 카드(부모 오브젝트 없음)만 수집
        foreach (var card in allCards)
            if (card.transform.parent == null)
                topCards.Add(card);

        // 각 최상위 카드에 대해 조합 조건을 확인
        foreach (var topCard in topCards)
        {
            // 자식이 없다면 스택이 아니므로 패스
            if (topCard.transform.childCount == 0)
                continue;

            // 스택의 가장 하단 카드 가져오기
            Card2D bottomCard = GetBottomCard(topCard);

            // 하단 카드에 Human 스크립트가 붙어있다면 조합 시도
            if (bottomCard != null && bottomCard.GetComponent<Human>() != null)
            {
                // 해당 스택의 모든 카드 수집
                List<Card2D> stackGroup = new List<Card2D>(topCard.GetComponentsInChildren<Card2D>());

                // 조합 시도
                TryCombine(stackGroup);
            }
        }
    }

    /// <summary>
    /// 최상위 카드에서 가장 아래에 있는 카드 반환
    /// </summary>
    private Card2D GetBottomCard(Card2D topCard)
    {
        Transform current = topCard.transform;
        Card2D lastCard = topCard;

        // 자식이 있는 한 계속 아래로 내려감
        while (current.childCount > 0)
        {
            current = current.GetChild(0); // 현재 오브젝트의 첫 번째 자식
            Card2D childCard = current.GetComponent<Card2D>();

            if (childCard != null)
                lastCard = childCard;
            else
                break;
        }

        return lastCard;
    }

    /// <summary>
    /// 카드 리스트로 조합을 시도하여 성공 시 새로운 카드 생성
    /// </summary>
    public CardData TryCombine(List<Card2D> cards)
    {
        List<Card2D> filteredCards = new List<Card2D>();
        Card2D triggerCard = null;

        foreach (var card in cards)
        {
            if (card.GetComponent<Human>() != null)
                triggerCard = card;
            else
                filteredCards.Add(card);
        }

        foreach (var recipe in recipes)
        {
            if (MatchRecipe(filteredCards, recipe))
            {
                Debug.Log("레시피 일치!");

                // 생존자 카드가 계층 구조 내에 있으면 부모에서 분리
                if (triggerCard != null)
                    triggerCard.transform.SetParent(null);

                // 나머지 카드 삭제
                foreach (var card in filteredCards)
                    Destroy(card.gameObject);

                // 새 카드 생성
                GameObject newCardObj = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                Card2D newCard = newCardObj.GetComponent<Card2D>();
                newCard.cardData = recipe.result;
                newCardObj.name = recipe.result.name;
                newCardObj.transform.SetParent(null);

                Debug.Log("새 카드 생성: " + recipe.result.name);
                return recipe.result;
            }
        }

        Debug.Log("일치하는 레시피 없음");
        return null;
    }

    /// <summary>
    /// 카드 리스트가 레시피와 정확히 일치하는지 확인
    /// </summary>
    private bool MatchRecipe(List<Card2D> inputCards, RecipeCardData recipe)
    {
        // 카드 종류별 개수를 세기
        var inputDict = new Dictionary<CardData, int>();
        foreach (var card in inputCards)
        {
            if (inputDict.ContainsKey(card.cardData))
                inputDict[card.cardData]++;
            else
                inputDict[card.cardData] = 1;
        }

        // 레시피 요구사항을 확인하며 차감
        foreach (var ingredient in recipe.ingredients)
        {
            var cardData = ingredient.ingredient;
            var requiredCount = ingredient.quantity;

            if (!inputDict.TryGetValue(cardData, out int currentCount) || currentCount < requiredCount)
                return false; // 카드가 없거나 수량 부족하면 실패

            // 수량 차감
            inputDict[cardData] -= requiredCount;

            // 다 사용했으면 제거
            if (inputDict[cardData] == 0)
                inputDict.Remove(cardData);
        }

        // 레시피에 없는 카드가 포함되어 있으면 실패
        return inputDict.Count == 0;
    }
}

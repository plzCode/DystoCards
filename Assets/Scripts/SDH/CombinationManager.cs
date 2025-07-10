using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 내 카드 조합을 관리하는 매니저 클래스
/// 최상위 카드 스택을 검사하여 유효한 조합이면 새 카드로 생성함
/// </summary>
public class CombinationManager : MonoBehaviour
{
    [SerializeField] private List<RecipeCardData> recipes; // 조합 가능한 레시피 목록
    [SerializeField] private GameObject cardPrefab;        // 조합 결과로 생성할 카드 프리팹

    private void Update()
    {
        // 씬에 존재하는 모든 Card2D 컴포넌트를 찾음
        Card2D[] allCards = FindObjectsByType<Card2D>(FindObjectsSortMode.None);
        List<Card2D> topCards = new List<Card2D>();

        // 부모가 없는 카드만 필터링 (즉, 스택의 최상단 카드)
        foreach (var card in allCards)
            if (card.transform.parent == null)
                topCards.Add(card);

        // 각 최상단 카드에 대해 조합 가능한 스택인지 확인
        foreach (var topCard in topCards)
        {
            // 자식이 없다면 스택이 아님
            if (topCard.transform.childCount == 0)
                continue;

            // 조합이 유효한 스택이면 조합 시도
            if (IsValidCombinationStack(topCard, out List<Card2D> stackGroup))
                TryCombine(stackGroup);
        }
    }

    // 조합이 가능한 스택인지 검사
    // 조건: Human 스크립트가 딱 1개 붙어 있는 경우
    private bool IsValidCombinationStack(Card2D topCard, out List<Card2D> stackGroup)
    {
        stackGroup = new List<Card2D>();
        int humanCount = 0;

        Transform current = topCard.transform;

        // 스택을 아래로 순회하면서 카드 수집
        while (current != null)
        {
            Card2D card = current.GetComponent<Card2D>();

            if (card == null)
                break;

            stackGroup.Add(card);

            // Human 스크립트가 붙어있으면 카운트 증가
            if (card.GetComponent<Human>() != null)
                humanCount++;

            // 다음 자식 카드로 이동
            current = current.childCount > 0 ? current.GetChild(0) : null;
        }

        // Human 스크립트가 딱 1개이고, 그 1개가 가장 아래 카드에 있을 때만 true
        return humanCount == 1 && stackGroup[^1].GetComponent<Human>() != null;
    }

    /// <summary>
    /// 카드 리스트로 조합을 시도하여 성공 시 새로운 카드를 생성합니다.
    /// </summary>
    /// <param name="cards">조합에 사용될 카드 리스트</param>
    public void TryCombine(List<Card2D> cards)
    {
        List<Card2D> filteredCards = new List<Card2D>(); // Human을 제외한 조합 대상 카드
        Card2D triggerCard = null;

        // 카드 중 Human 스크립트를 가진 카드는 트리거로 분류
        foreach (var card in cards)
        {
            if (card.GetComponent<Human>() != null)
                triggerCard = card;
            else
                filteredCards.Add(card);
        }

        // 각 레시피와 비교하여 조합 가능한지 확인
        foreach (var recipe in recipes)
        {
            if (MatchRecipe(filteredCards, recipe))
            {
                Debug.Log("레시피 일치!");

                // Human 카드를 부모에서 분리 (스택 해제)
                if (triggerCard != null)
                    triggerCard.transform.SetParent(null);

                // 나머지 카드 파괴
                foreach (var card in filteredCards)
                    Destroy(card.gameObject);

                // 결과 카드 생성
                GameObject newCardObj = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                Card2D newCard = newCardObj.GetComponent<Card2D>();
                newCard.cardData = recipe.result;
                newCardObj.name = recipe.result.name;
                newCardObj.transform.SetParent(null);

                Debug.Log("새 카드 생성: " + recipe.result.name);
            }
        }

        // 일치하는 레시피가 없을 경우
        Debug.Log("일치하는 레시피 없음");
    }

    /// <summary>
    /// 주어진 카드 리스트가 특정 레시피와 정확히 일치하는지 확인합니다.
    /// </summary>
    /// <param name="inputCards">현재 조합 시도 중인 카드들</param>
    /// <param name="recipe">비교할 레시피</param>
    /// <returns>일치하면 true, 아니면 false</returns>
    private bool MatchRecipe(List<Card2D> inputCards, RecipeCardData recipe)
    {
        // 카드 종류별 개수를 딕셔너리에 저장
        var inputDict = new Dictionary<CardData, int>();
        foreach (var card in inputCards)
        {
            if (inputDict.ContainsKey(card.cardData))
                inputDict[card.cardData]++;
            else
                inputDict[card.cardData] = 1;
        }

        // 레시피 요구사항과 비교
        foreach (var ingredient in recipe.ingredients)
        {
            var cardData = ingredient.ingredient;
            var requiredCount = ingredient.quantity;

            // 해당 카드가 없거나 수량 부족한 경우 실패
            if (!inputDict.TryGetValue(cardData, out int currentCount) || currentCount < requiredCount)
                return false;

            // 수량 차감
            inputDict[cardData] -= requiredCount;

            // 다 썼으면 딕셔너리에서 제거
            if (inputDict[cardData] == 0)
                inputDict.Remove(cardData);
        }

        // 레시피 외의 카드가 더 있으면 실패
        return inputDict.Count == 0;
    }
}

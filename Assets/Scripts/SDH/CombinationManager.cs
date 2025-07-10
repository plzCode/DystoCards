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
        // 씬 내 모든 Card2D 컴포넌트를 찾아서 배열로 가져옴
        Card2D[] allCards = FindObjectsByType<Card2D>(FindObjectsSortMode.None);
        List<Card2D> topCards = new List<Card2D>();

        // 부모가 없는 카드만 필터링 -> 스택의 최상위 카드들만 선별
        foreach (var card in allCards)
            if (card.transform.parent == null)
                topCards.Add(card);

        // 각 최상위 카드별로 조합 가능한 스택인지 확인 후 조합 시도
        foreach (var topCard in topCards)
        {
            // 자식 카드가 없으면 스택이 아니므로 건너뜀
            if (topCard.transform.childCount == 0)
                continue;

            // 유효한 조합 스택인지 검사, 결과로 스택에 포함된 카드 리스트 반환
            if (IsValidCombinationStack(topCard, out List<Card2D> stackGroup))
                TryCombine(stackGroup); // 조합 시도
        }
    }

    // 조합 가능한 카드 스택인지 검사하는 함수
    // 조건: Human 카드가 딱 1개 포함되어 있어야 함
    private bool IsValidCombinationStack(Card2D topCard, out List<Card2D> stackGroup)
    {
        stackGroup = new List<Card2D>();
        int humanCount = 0;

        Transform current = topCard.transform;

        // 스택을 위에서 아래로 따라가며 카드들을 수집
        while (current != null)
        {
            Card2D card = current.GetComponent<Card2D>();

            // Card2D 컴포넌트가 없으면 종료
            if (card == null)
                break;

            stackGroup.Add(card);

            // 해당 카드가 Human 타입인지 검사
            if (card.IsCharacterOfType(card.cardData, CharacterType.Human))
                humanCount++;

            // 자식이 있으면 첫 번째 자식으로 이동, 없으면 null로 루프 종료
            current = current.childCount > 0 ? current.GetChild(0) : null;
        }

        // Human 카드가 정확히 1개이고 그 카드가 스택의 가장 아래(마지막)에 있을 때만 true 반환
        return humanCount == 1 && stackGroup[^1].IsCharacterOfType(stackGroup[^1].cardData, CharacterType.Human);
    }

    /// <summary>
    /// 카드 리스트로 조합을 시도하여 성공 시 새로운 카드를 생성합니다.
    /// </summary>
    /// <param name="cards">조합에 사용될 카드 리스트</param>
    public void TryCombine(List<Card2D> cards)
    {
        List<Card2D> filteredCards = new List<Card2D>(); // Human 카드를 제외한 조합 대상 카드 리스트
        Card2D triggerCard = null; // Human 카드를 따로 저장

        // 카드 중 Human 카드는 트리거 역할로 분리, 나머지는 조합 대상에 추가
        foreach (var card in cards)
        {
            if (card.IsCharacterOfType(card.cardData, CharacterType.Human))
                triggerCard = card;
            else
                filteredCards.Add(card);
        }

        // 등록된 모든 레시피와 비교하여 조합 가능한지 검사
        foreach (var recipe in recipes)
        {
            if (MatchRecipe(filteredCards, recipe))
            {
                Debug.Log("레시피 일치!");

                // Human 카드를 부모에서 분리해 스택 해제
                if (triggerCard != null)
                    triggerCard.transform.SetParent(null);

                // 나머지 카드들은 모두 파괴
                foreach (var card in filteredCards)
                    Destroy(card.gameObject);

                // 조합 결과 카드 생성 및 초기화
                GameObject newCardObj = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                Card2D newCard = newCardObj.GetComponent<Card2D>();
                newCard.cardData = recipe.result;
                newCardObj.name = recipe.result.name;
                newCardObj.transform.SetParent(null);

                Debug.Log("새 카드 생성: " + recipe.result.name);
            }
        }

        // 일치하는 레시피가 없으면 로그 출력
        Debug.Log("일치하는 레시피 없음");
    }

    /// <summary>
    /// 주어진 카드 리스트가 특정 레시피와 정확히 일치하는지 확인합니다.
    /// </summary>
    /// <param name="inputCards">현재 조합 시도 중인 카드 리스트</param>
    /// <param name="recipe">비교할 레시피 데이터</param>
    /// <returns>레시피와 일치하면 true, 아니면 false</returns>
    private bool MatchRecipe(List<Card2D> inputCards, RecipeCardData recipe)
    {
        // 입력 카드들을 카드 종류별 개수로 집계
        var inputDict = new Dictionary<CardData, int>();
        foreach (var card in inputCards)
        {
            if (inputDict.ContainsKey(card.cardData))
                inputDict[card.cardData]++;
            else
                inputDict[card.cardData] = 1;
        }

        // 레시피의 각 재료별 요구 수량과 비교
        foreach (var ingredient in recipe.ingredients)
        {
            var cardData = ingredient.ingredient;
            var requiredCount = ingredient.quantity;

            // 해당 카드가 없거나 요구 수량보다 적으면 실패
            if (!inputDict.TryGetValue(cardData, out int currentCount) || currentCount < requiredCount)
                return false;

            // 사용한 수량 차감
            inputDict[cardData] -= requiredCount;

            // 사용량이 0이면 딕셔너리에서 제거
            if (inputDict[cardData] == 0)
                inputDict.Remove(cardData);
        }

        // 남은 카드가 있으면 레시피와 불일치 -> 실패
        return inputDict.Count == 0;
    }
}

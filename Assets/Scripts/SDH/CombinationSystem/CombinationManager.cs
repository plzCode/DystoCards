using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 게임 내 카드 조합을 관리하는 매니저 클래스
/// 최상위 카드 스택을 검사하여 유효한 조합이면 새 카드로 생성함
/// </summary>
public class CombinationManager : MonoBehaviour
{
    [SerializeField] private List<RecipeCardData> recipes; // 조합 가능한 레시피 목록
    [SerializeField] private GameObject fieldCards;        // 필드에 놓인 카드들의 부모 오브젝트

    public static CombinationManager Instance { get; private set; }

    private void Awake()
    {
        // 싱글톤 인스턴스 지정
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬이 넘어가도 유지
    }

    public void CheckCombination()
    {
        // 씬 내 존재하는 모든 Card2D 컴포넌트를 찾아서 배열로 가져옴
        Card2D[] allCards = FindObjectsByType<Card2D>(FindObjectsSortMode.None);
        List<Card2D> topCards = new List<Card2D>(); // 최상위 카드들(스택의 맨 위 카드) 저장용 리스트

        // 모든 카드 중에서 부모가 fieldCards인 경우만 골라서 topCards에 추가
        foreach (var card in allCards)
            if (card.transform.parent == fieldCards.transform)
                topCards.Add(card);

        // 각 최상위 카드별로 조합이 가능한 스택인지 검사하고, 유효하면 조합 시도
        foreach (var topCard in topCards)
        {
            // 자식 카드가 없으면 단일 카드이므로 조합 검사에서 제외
            if (topCard.transform.childCount == 0)
                continue;

            // 유효한 조합 스택인지 검사 -> 조건에 맞으면 스택을 반환
            if (IsValidCombinationStack(topCard, out List<Card2D> stackGroup))
                TryCombine(stackGroup); // 조합 시도
        }
    }

    /// <summary>
    /// 조합 가능한 카드 스택인지 검사하는 함수
    /// 조건: Human 카드가 딱 1개 포함되어 있으며, 반드시 스택의 마지막(맨 아래)에 위치해야 함
    /// </summary>
    /// <param name="topCard">스택의 최상위 카드</param>
    /// <param name="stackGroup">스택에 포함된 카드들을 반환</param>
    /// <returns>조건을 만족하면 true</returns>
    private bool IsValidCombinationStack(Card2D topCard, out List<Card2D> stackGroup)
    {
        stackGroup = new List<Card2D>();
        int humanCount = 0;

        Transform current = topCard.transform;

        // 스택을 위에서 아래로 순회하며 카드들을 수집
        while (current != null)
        {
            Card2D card = current.GetComponent<Card2D>();

            // Card2D가 없으면 스택 순회 종료
            if (card == null)
                break;

            stackGroup.Add(card);

            // Human 타입인지 확인
            if (card.IsCharacterOfType(card.cardData, CharacterType.Human))
                humanCount++;

            // 다음 카드로 이동 (자식이 있으면 첫 번째 자식으로, 없으면 null)
            //current = current.childCount > 0 ? current.GetChild(0) : null;
            if(card.childCards != null && card.childCards.Count > 0)
            {
                current = card.childCards[0].transform; // 첫 번째 자식 카드로 이동
            }
            else
                current = null; // 더 이상 자식이 없으면 순회 종료            
        }

        // 디버그 출력으로 스택 상태를 확인
        Debug.Log($"[IsValidCombinationStack] 스택 검사: {topCard.name}");
        Debug.Log($"- 스택 카드 수: {stackGroup.Count}");
        Debug.Log($"- Human 카드 개수: {humanCount}");
        Debug.Log($"- 마지막 카드: {stackGroup[^1].name}");

        // 스택의 마지막 카드가 Human인지 확인
        bool lastCardIsHuman = stackGroup[^1].IsCharacterOfType(stackGroup[^1].cardData, CharacterType.Human);
        Debug.Log($"- 마지막 카드가 Human인가? {lastCardIsHuman}");

        // 조건: Human 카드가 딱 1개이며, 스택의 마지막 카드여야 함
        return humanCount == 1 && lastCardIsHuman;
    }

    /// <summary>
    /// 카드 리스트를 기반으로 조합을 시도하고, 성공 시 새 카드를 생성함
    /// </summary>
    /// <param name="cards">조합에 사용될 카드 리스트</param>
    public void TryCombine(List<Card2D> cards)
    {
        List<Card2D> filteredCards = new List<Card2D>(); // Human 카드를 제외한 조합 대상 카드들
        Card2D triggerCard = null; // Human 카드 저장용 (조합의 트리거 역할)

        // Human 카드를 따로 저장하고 나머지 카드만 조합 대상으로 분류
        foreach (var card in cards)
        {
            if (card.IsCharacterOfType(card.cardData, CharacterType.Human))
                triggerCard = card;
            else
                filteredCards.Add(card);
        }

        // 모든 레시피를 순회하며 조합 조건이 맞는지 확인
        foreach (var recipe in recipes)
        {
            if (MatchRecipe(filteredCards, recipe))
            {
                Debug.Log("레시피 일치!");

                // Human 카드를 스택에서 분리 (자식 관계 해제)
                if (triggerCard != null)
                    triggerCard.transform.SetParent(null);

                // 조합에 사용된 카드(재료 카드)들을 모두 파괴
                foreach (var card in filteredCards)
                    CardManager.Instance.DestroyCard(card);

                if (triggerCard != null)
                {
                    // 새 카드 생성 위치 계산 (Human 카드 기준 약간 아래쪽으로)
                    SpriteRenderer triggerRenderer = triggerCard.GetComponent<SpriteRenderer>();
                    Vector3 spawnPosition = triggerCard.transform.position;
                    spawnPosition.y -= 0.2f;

                    // 새로운 카드 생성
                    Card2D newCard = CardManager.Instance.SpawnCard(recipe.result, spawnPosition);
                    newCard.BringToFrontRecursive(newCard);
                    newCard.cardAnim.PlayFeedBack_ByName("BounceY");

                    // 새 카드의 부모를 fieldCards로 설정
                    newCard.transform.SetParent(fieldCards.transform);

                    // newCard localPosition.z 0으로 설정
                    Vector3 newLocalPos = newCard.transform.localPosition;
                    newLocalPos.z = 0f;
                    newCard.transform.localPosition = newLocalPos;

                    // triggerCard localPosition.z 0으로 설정
                    Vector3 triggerLocalPos = triggerCard.transform.localPosition;
                    triggerLocalPos.z = 0f;
                    triggerCard.transform.localPosition = triggerLocalPos;

                    // 렌더링 순서 조정 (Human 카드보다 위에 보이도록)
                    SpriteRenderer newCardRenderer = newCard.GetComponent<SpriteRenderer>();
                    if (triggerRenderer != null && newCardRenderer != null)
                    {
                        newCardRenderer.sortingLayerName = triggerRenderer.sortingLayerName;
                        newCardRenderer.sortingOrder = triggerRenderer.sortingOrder + 1;
                    }

                    string scriptName = recipe.scriptName;

                    if (!string.IsNullOrEmpty(scriptName))
                    {
                        Type type = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(a => a.GetTypes())
                            .FirstOrDefault(t => t.Name == scriptName || t.FullName == scriptName);

                        if (type != null && typeof(MonoBehaviour).IsAssignableFrom(type))
                        {
                            newCard.gameObject.AddComponent(type);
                            Debug.Log($"스크립트 부착 완료: {scriptName}");
                        }
                        else
                        {
                            Debug.LogError($"스크립트 '{scriptName}' 를 찾을 수 없습니다. 클래스명이 정확한지 확인해주세요.");
                        }
                    }

                    Debug.Log($"newCard.cardData.cardName: {newCard.RuntimeData.cardName}");
                    Debug.Log("새 카드 생성: " + recipe.result.name);
                }

                // Human 카드의 스테미나 감소 처리
                Human human = triggerCard.GetComponent<Human>();
                if (human != null)
                {
                    human.ConsumeStamina(1);
                }

                return; // 조합 성공 시 함수 종료
            }
        }

        // 일치하는 레시피가 없을 경우 로그 출력
        Debug.Log("일치하는 레시피 없음");
    }

    /// <summary>
    /// 현재 카드 리스트가 주어진 레시피와 정확히 일치하는지 확인
    /// </summary>
    /// <param name="inputCards">조합에 사용된 카드들 (Human 제외)</param>
    /// <param name="recipe">검사할 레시피</param>
    /// <returns>일치하면 true</returns>
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

        // 레시피 재료와 비교
        foreach (var ingredient in recipe.ingredients)
        {
            var cardData = ingredient.ingredient;
            var requiredCount = ingredient.quantity;

            // 요구하는 카드가 없거나, 개수가 부족하면 일치하지 않음
            if (!inputDict.TryGetValue(cardData, out int currentCount) || currentCount < requiredCount)
                return false;

            // 사용한 재료는 딕셔너리에서 개수 차감
            inputDict[cardData] -= requiredCount;

            if (inputDict[cardData] == 0)
                inputDict.Remove(cardData);
        }

        // 모든 재료를 정확히 사용했는지 확인
        // 남은 카드가 있으면 불일치
        return inputDict.Count == 0;
    }
}

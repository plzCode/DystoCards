﻿using System;
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
    public float currentReaminingTime;



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

    /// <summary>
    /// 현재 필드 상의 모든 카드 중 최상위 카드 스택을 검사하여 조합 가능한 경우 조합을 시도
    /// </summary>
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
        int techCount = 0;

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

            if (card.IsTechOfType(card.cardData, TechType.Tech))
                techCount++;

            // 다음 카드로 이동 (자식이 있으면 첫 번째 자식으로, 없으면 null)
            //current = current.childCount > 0 ? current.GetChild(0) : null;
            if (card.childCards != null && card.childCards.Count > 0)
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
        Debug.Log($"- Tech 카드 개수: {techCount}");
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
        Card2D techCard = null; // Tech 카드 저장용 (조합에 사용될 수 있음)

        // Human 카드를 따로 저장하고 나머지 카드만 조합 대상으로 분류
        foreach (var card in cards)
        {
            if (card.IsCharacterOfType(card.cardData, CharacterType.Human))
                triggerCard = card;
            else if (card.IsTechOfType(card.cardData, TechType.Tech))
            {
                techCard = card;
                filteredCards.Add(card);
            }
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
                if (techCard != null)
                    techCard.transform.SetParent(null);

                // 조합에 사용된 카드(재료 카드)들을 모두 파괴
                foreach (var card in filteredCards)
                    CardManager.Instance.DestroyCard(card);

                RecipeCardData recipeToReplace = recipe;

                if (triggerCard != null)
                {
                    if (techCard != null)
                    {
                        //// Tech 카드가 있다면 해당 카드도 파괴
                        //TechCardData techCardData = techCard.cardData as TechCardData;
                        //currentReaminingTime = techCardData.remainingTime;


                        //TechCardData techCardData = techCard.cardData as TechCardData;
                        //int currentRemainingTime = GetRemaining(techCard, techCardData);

                        TechCardData techCardData = techCard.cardData as TechCardData;
                        if (techCardData == null)
                        {
                            Debug.LogError("[TryCombine] TechCardData 캐스팅 실패");
                            return;
                        }

                        // 초기화 (처음이면 남은일수 세팅)
                        if (!_techRemaining.ContainsKey(techCardData))
                        {
                            SetRemaining(techCardData, Mathf.Max(1, techCardData.remainingTime));
                            Debug.Log($"[TryCombine] 초기화: {techCardData.name} = {GetRemaining(techCardData)}일");
                        }
                        else
                        {
                            Debug.Log($"[TryCombine] 이미 진행 중: {techCardData.name} = {GetRemaining(techCardData)}일");
                        }


                        bool added = _techTickingTech.Add(techCardData);
                        if (!added) return;

                        Action onceAction = null;
                        onceAction = () =>
                        {
                            //// 먼저 등록 해제: 이번 턴 한 번만 실행
                            //TurnManager.Instance.UnregisterPhaseAction(TurnPhase.ExploreAction, onceAction);

                            //techCardData.remainingTime -= 1;
                            //if (techCardData.remainingTime < 0) techCardData.remainingTime = 0;
                            //Debug.Log($"[CombinationManager] 기술 시간 감소: {techCardData.remainingTime}");

                            //if (techCardData.remainingTime <= 0)
                            //{
                            //    //if (recipeToReplace != null && recipes.Remove(recipeToReplace))
                            //    //    Debug.Log($"[CombinationManager] 기존 레시피 삭제: {recipeToReplace.cardName}");

                            //    if (techCardData.unlockRecipe != null && !recipes.Contains(techCardData.unlockRecipe))
                            //    {
                            //        recipes.Add(techCardData.unlockRecipe);
                            //        Debug.Log($"[CombinationManager] 언락 레시피 추가: {techCardData.unlockRecipe.cardName}");
                            //        GradeRecorder.Instance.recipeOpenCount++; // 레시피오픈  횟수 증가
                            //    }
                            //}


                            //currentReaminingTime -= 1;
                            //if (currentReaminingTime < 0) currentReaminingTime = 0;
                            //Debug.Log($"[CombinationManager] 기술 시간 감소: {techCardData.remainingTime}");

                            //if (currentReaminingTime <= 0)
                            //{
                            //    //if (recipeToReplace != null && recipes.Remove(recipeToReplace))
                            //    //    Debug.Log($"[CombinationManager] 기존 레시피 삭제: {recipeToReplace.cardName}");

                            //    if (techCardData.unlockRecipe != null && !recipes.Contains(techCardData.unlockRecipe))
                            //    {
                            //        recipes.Add(techCardData.unlockRecipe);
                            //        Debug.Log($"[CombinationManager] 언락 레시피 추가: {techCardData.unlockRecipe.cardName}");
                            //        GradeRecorder.Instance.recipeOpenCount++; // 레시피오픈  횟수 증가
                            //    }
                            //}

                            //currentRemainingTime = Mathf.Max(0, currentRemainingTime - 1);
                            //SetRemaining(techCard, currentRemainingTime);

                            //Debug.Log($"[CombinationManager] 기술 시간 감소: {currentRemainingTime}");

                            //if (currentRemainingTime <= 0)
                            //{
                            //    if (techCardData.unlockRecipe != null && !recipes.Contains(techCardData.unlockRecipe))
                            //    {
                            //        recipes.Add(techCardData.unlockRecipe);
                            //        Debug.Log($"[CombinationManager] 언락 레시피 추가: {techCardData.unlockRecipe.cardName}");
                            //        GradeRecorder.Instance.recipeOpenCount++;
                            //    }

                            //    // 다 썼으면 메모리 정리(선택)
                            //    _techRemaining.Remove(techCard);
                            //    // 필요 시 카드 파괴:
                            //    // CardManager.Instance.DestroyCard(techCard);
                            //}

                            //currentReaminingTime = Mathf.Max(0, currentReaminingTime - 1);
                            //SetRemaining(techCard, currentReaminingTime);

                            //Debug.Log($"[CombinationManager] 기술 시간 감소: {currentRemainingTime}");


                            int cur = GetRemaining(techCard, techCardData);
                            cur = Mathf.Max(0, cur - 1);
                            SetRemaining(techCard, cur);

                            Debug.Log($"[CombinationManager] {techCardData.name} 기술 시간 감소 → {cur}");


                            if (cur <= 0)
                            {
                                if (techCardData.unlockRecipe != null && !recipes.Contains(techCardData.unlockRecipe))
                                {
                                    recipes.Add(techCardData.unlockRecipe);
                                    Debug.Log($"[CombinationManager] 언락 레시피 추가: {techCardData.unlockRecipe.cardName}");
                                    GradeRecorder.Instance.recipeOpenCount++;
                                }
                                // 완료: 메모리 정리
                                _techRemaining.Remove(techCardData);
                                // 필요시 카드 처리:
                                // CardManager.Instance.DestroyCard(techCard);
                            }
                            else
                            {
                                // ⬅️ 아직 남았으면 다음 ExploreAction 때 다시 실행되도록 재등록
                                TurnManager.Instance.RegisterPhaseAction(TurnPhase.ExploreAction, onceAction);
                            }


                        };
                        TurnManager.Instance.RegisterPhaseAction(TurnPhase.ExploreAction, onceAction);
                        return;
                    }




                    // 새 카드 생성 위치 계산 (Human 카드 기준 약간 아래쪽으로)
                    SpriteRenderer triggerRenderer = triggerCard.GetComponent<SpriteRenderer>();
                    Vector3 spawnPosition = triggerCard.transform.position;
                    spawnPosition.y -= 0.2f;

                    // 새로운 카드 생성
                    Card2D newCard = CardManager.Instance.SpawnCard(recipe.result, spawnPosition);
                    newCard.BringToFrontRecursive(newCard); // 카드가 위에 보이도록 정렬
                    newCard.cardAnim.PlayFeedBack_ByName("BounceY"); // 생성 애니메이션 실행

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

                    Debug.Log("새 카드 생성: " + recipe.result.name);
                    GradeRecorder.Instance.combinationCount++; // 조합 횟수 증가
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



    // 외부에서는 읽기 전용으로 접근
    public IReadOnlyList<RecipeCardData> Recipes => recipes;

    public event Action<RecipeCardData> OnRecipeAdded;
    public event Action<RecipeCardData> OnRecipeRemoved;

    public bool HasRecipe(RecipeCardData r) => r != null && recipes.Contains(r);

    public bool AddRecipeUnique(RecipeCardData r)
    {
        if (r == null || recipes.Contains(r)) return false;
        recipes.Add(r);
        OnRecipeAdded?.Invoke(r);
        return true;

    }

    // 외부에서 추가할 때 사용할 메서드
    public void RecipesInternalAdd(RecipeCardData r)
    {
        if (r != null && !recipes.Contains(r))
            recipes.Add(r);
    }

    public bool RemoveRecipe(RecipeCardData r)
    {
        if (r == null) return false;
        bool removed = recipes.Remove(r);
        if (removed) OnRecipeRemoved?.Invoke(r);
        return removed;
    }


    private readonly Dictionary<TechCardData, int> _techRemaining = new();
    // 중복 등록 가드 (Tech 기준)
    private readonly HashSet<TechCardData> _techTickingTech = new();



    private int GetRemaining(TechCardData data)
    {
        if (data == null) return 0;
        if (!_techRemaining.TryGetValue(data, out var v))
        {
            v = Mathf.Max(1, data.remainingTime);   // 최초 진입 시 초기화
            _techRemaining[data] = v;
        }
        return v;
    }

    private void SetRemaining(TechCardData data, int value)
    {
        if (data == null) return;
        _techRemaining[data] = Mathf.Max(0, value);
    }

    // (호환용) 기존에 Card2D 버전 호출이 남아있다면 임시 포워딩
    private int GetRemaining(Card2D _, TechCardData data) => GetRemaining(data);
    private void SetRemaining(Card2D card, int value)
    {
        TechCardData data = card != null ? card.cardData as TechCardData : null;
        SetRemaining(data, value);
    }

}
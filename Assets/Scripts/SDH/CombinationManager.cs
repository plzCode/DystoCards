using System.Collections.Generic;
using UnityEngine;

public class CombinationManager : MonoBehaviour
{
    public List<CardRecipe> recipes;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) // T 키 누르면 테스트 실행
        {
            TestCombination();
        }
    }

    [SerializeField] GameObject cardPrefab; // Card 컴포넌트가 붙은 프리팹

    public CardData TryCombine(List<Card> cards)
    {
        foreach (var recipe in recipes)
        {
            if (MatchRecipe(cards, recipe))
            {
                Debug.Log("레시피 일치!");

                // 기존 카드 제거
                foreach (var card in cards)
                {
                    Destroy(card.gameObject);
                }

                // 새 카드 생성
                GameObject newCardObj = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                Card newCard = newCardObj.GetComponent<Card>();
                newCard.data = recipe.resultCard;

                // 오브젝트 이름을 카드 데이터 이름으로 설정
                newCardObj.name = recipe.resultCard.name;

                Debug.Log("새 카드 생성: " + recipe.resultCard.name);
                return recipe.resultCard;
            }
        }

        return null;
    }

    private bool MatchRecipe(List<Card> inputCards, CardRecipe recipe)
    {
        var input = new List<CardData>();
        foreach (var c in inputCards)
            input.Add(c.data);

        var required = new List<CardData>(recipe.requiredCards);

        // 모든 카드가 일치하는지 확인
        foreach (var data in required)
        {
            if (input.Contains(data))
                input.Remove(data);
            else
                return false;
        }

        return input.Count == 0;
    }

    [SerializeField] private Card cardDataA;
    [SerializeField] private Card cardDataB;

    public void TestCombination()
    {
        // 임의 카드 리스트 생성 (예: cardDataA, cardDataB 카드 2장)
        List<Card> testCards = new List<Card>()
        {
            cardDataA,
            cardDataB
        };

        // 조합 시도
        CardData result = TryCombine(testCards);

        if (result != null)
        {
            Debug.Log("조합 성공! 결과 카드: " + result.name);
        }
        else
        {
            Debug.Log("조합 실패");
        }
    }
}

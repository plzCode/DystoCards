using System.Collections.Generic;
using UnityEngine;

public class CombinationManager : MonoBehaviour
{
    public List<CardRecipe> recipes;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) // T Ű ������ �׽�Ʈ ����
        {
            TestCombination();
        }
    }

    [SerializeField] GameObject cardPrefab; // Card ������Ʈ�� ���� ������

    public CardData TryCombine(List<Card> cards)
    {
        foreach (var recipe in recipes)
        {
            if (MatchRecipe(cards, recipe))
            {
                Debug.Log("������ ��ġ!");

                // ���� ī�� ����
                foreach (var card in cards)
                {
                    Destroy(card.gameObject);
                }

                // �� ī�� ����
                GameObject newCardObj = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                Card newCard = newCardObj.GetComponent<Card>();
                newCard.data = recipe.resultCard;

                // ������Ʈ �̸��� ī�� ������ �̸����� ����
                newCardObj.name = recipe.resultCard.name;

                Debug.Log("�� ī�� ����: " + recipe.resultCard.name);
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

        // ��� ī�尡 ��ġ�ϴ��� Ȯ��
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
        // ���� ī�� ����Ʈ ���� (��: cardDataA, cardDataB ī�� 2��)
        List<Card> testCards = new List<Card>()
        {
            cardDataA,
            cardDataB
        };

        // ���� �õ�
        CardData result = TryCombine(testCards);

        if (result != null)
        {
            Debug.Log("���� ����! ��� ī��: " + result.name);
        }
        else
        {
            Debug.Log("���� ����");
        }
    }
}

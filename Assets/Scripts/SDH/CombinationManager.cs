using System.Collections.Generic;
using UnityEngine;

public class CombinationManager : MonoBehaviour
{
    public List<RecipeCardData> recipes; // ���� ������ ������ ���
    [SerializeField] private GameObject cardPrefab; // ��� ī�� ������

    private void Update()
    {
        // ���� �����ϴ� ��� ī�� �߿��� �ֻ��� ī��(�θ� ����)�� ����
        Card2D[] allCards = FindObjectsByType<Card2D>(FindObjectsSortMode.None);
        List<Card2D> topCards = new List<Card2D>();

        foreach (var card in allCards)
        {
            if (card.transform.parent == null)
            {
                topCards.Add(card);
            }
        }

        // �� �ֻ��� ī�忡 ���� ���� ������ Ȯ��
        foreach (var topCard in topCards)
        {
            // �ڽ��� ���ٸ� ������ �ƴϹǷ� �н�
            if (topCard.transform.childCount == 0)
                continue;

            // ������ ���� �ϴ� ī�� ��������
            Card2D bottomCard = GetBottomCard(topCard);

            // �ϴ� ī�尡 Ʈ���� ī��(id == "000")��� ���� �õ�
            if (bottomCard != null && bottomCard.cardData.cardId == "000")
            {
                // �ش� ������ ��� ī�� ����
                List<Card2D> stackGroup = new List<Card2D>(topCard.GetComponentsInChildren<Card2D>());

                Debug.Log($"���� �õ�: {topCard.name} ����");

                // ���� �õ�
                TryCombine(stackGroup);
            }
        }
    }

    /// <summary>
    /// �ֻ��� ī�忡�� ���� �Ʒ��� �ִ� ī�� ��ȯ
    /// </summary>
    private Card2D GetBottomCard(Card2D topCard)
    {
        Transform current = topCard.transform;
        Card2D lastCard = topCard;

        // �ڽ��� �ִ� �� ��� �Ʒ��� ������
        while (current.childCount > 0)
        {
            current = current.GetChild(0);
            Card2D childCard = current.GetComponent<Card2D>();
            if (childCard != null)
                lastCard = childCard;
            else
                break;
        }

        return lastCard;
    }

    /// <summary>
    /// ī�� ����Ʈ�� ������ �õ��Ͽ� ���� �� ���ο� ī�� ����
    /// </summary>
    public CardData TryCombine(List<Card2D> cards)
    {
        foreach (var recipe in recipes)
        {
            if (MatchRecipe(cards, recipe))
            {
                Debug.Log("������ ��ġ!");

                // ���� ī�� �ı�
                foreach (var card in cards)
                {
                    Destroy(card.gameObject);
                }

                // ���ο� ī�� ����
                GameObject newCardObj = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                Card2D newCard = newCardObj.GetComponent<Card2D>();
                newCard.cardData = recipe.result;
                newCardObj.name = recipe.result.name;

                // �ֻ����� ���� (�θ� ����)
                newCardObj.transform.SetParent(null);

                Debug.Log("�� ī�� ����: " + recipe.result.name);
                return recipe.result;
            }
        }

        Debug.Log("��ġ�ϴ� ������ ����");
        return null;
    }

    /// <summary>
    /// ī�� ����Ʈ�� �����ǿ� ��Ȯ�� ��ġ�ϴ��� Ȯ��
    /// </summary>
    private bool MatchRecipe(List<Card2D> inputCards, RecipeCardData recipe)
    {
        // ī�� ������ ������ ����
        var inputDict = new Dictionary<CardData, int>();
        foreach (var card in inputCards)
        {
            if (inputDict.ContainsKey(card.cardData))
                inputDict[card.cardData]++;
            else
                inputDict[card.cardData] = 1;
        }

        foreach (var kvp in inputDict)
            Debug.Log($"�Է� ī��: {kvp.Key.name}, ����: {kvp.Value}");

        foreach (var ing in recipe.ingredients)
            Debug.Log($"������ �ʿ�: {ing.ingredient.name}, ����: {ing.quantity}");

        // ������ �䱸������ Ȯ���ϸ� ����
        foreach (var ingredient in recipe.ingredients)
        {
            var cardData = ingredient.ingredient;
            var requiredCount = ingredient.quantity;

            if (!inputDict.TryGetValue(cardData, out int currentCount) || currentCount < requiredCount)
                return false; // ī�尡 ���ų� ���� �����ϸ� ����

            // ���� ����
            inputDict[cardData] -= requiredCount;

            // �� ��������� ����
            if (inputDict[cardData] == 0)
                inputDict.Remove(cardData);
        }

        // �����ǿ� ���� ī�尡 ���ԵǾ� ������ ����
        return inputDict.Count == 0;
    }
}

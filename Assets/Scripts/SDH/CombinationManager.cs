using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� �� ī�� ������ �����ϴ� �Ŵ��� Ŭ����
/// �ֻ��� ī�� ������ �˻��Ͽ� ��ȿ�� �����̸� �� ī��� ������
/// </summary>
public class CombinationManager : MonoBehaviour
{
    [SerializeField] private List<RecipeCardData> recipes; // ���� ������ ������ ���
    [SerializeField] private GameObject cardPrefab;        // ���� ����� ������ ī�� ������

    private void Update()
    {
        // ���� �����ϴ� ��� Card2D ������Ʈ�� ã��
        Card2D[] allCards = FindObjectsByType<Card2D>(FindObjectsSortMode.None);
        List<Card2D> topCards = new List<Card2D>();

        // �θ� ���� ī�常 ���͸� (��, ������ �ֻ�� ī��)
        foreach (var card in allCards)
            if (card.transform.parent == null)
                topCards.Add(card);

        // �� �ֻ�� ī�忡 ���� ���� ������ �������� Ȯ��
        foreach (var topCard in topCards)
        {
            // �ڽ��� ���ٸ� ������ �ƴ�
            if (topCard.transform.childCount == 0)
                continue;

            // ������ ��ȿ�� �����̸� ���� �õ�
            if (IsValidCombinationStack(topCard, out List<Card2D> stackGroup))
                TryCombine(stackGroup);
        }
    }

    // ������ ������ �������� �˻�
    // ����: Human ��ũ��Ʈ�� �� 1�� �پ� �ִ� ���
    private bool IsValidCombinationStack(Card2D topCard, out List<Card2D> stackGroup)
    {
        stackGroup = new List<Card2D>();
        int humanCount = 0;

        Transform current = topCard.transform;

        // ������ �Ʒ��� ��ȸ�ϸ鼭 ī�� ����
        while (current != null)
        {
            Card2D card = current.GetComponent<Card2D>();

            if (card == null)
                break;

            stackGroup.Add(card);

            // Human ��ũ��Ʈ�� �پ������� ī��Ʈ ����
            if (card.GetComponent<Human>() != null)
                humanCount++;

            // ���� �ڽ� ī��� �̵�
            current = current.childCount > 0 ? current.GetChild(0) : null;
        }

        // Human ��ũ��Ʈ�� �� 1���̰�, �� 1���� ���� �Ʒ� ī�忡 ���� ���� true
        return humanCount == 1 && stackGroup[^1].GetComponent<Human>() != null;
    }

    /// <summary>
    /// ī�� ����Ʈ�� ������ �õ��Ͽ� ���� �� ���ο� ī�带 �����մϴ�.
    /// </summary>
    /// <param name="cards">���տ� ���� ī�� ����Ʈ</param>
    public void TryCombine(List<Card2D> cards)
    {
        List<Card2D> filteredCards = new List<Card2D>(); // Human�� ������ ���� ��� ī��
        Card2D triggerCard = null;

        // ī�� �� Human ��ũ��Ʈ�� ���� ī��� Ʈ���ŷ� �з�
        foreach (var card in cards)
        {
            if (card.GetComponent<Human>() != null)
                triggerCard = card;
            else
                filteredCards.Add(card);
        }

        // �� �����ǿ� ���Ͽ� ���� �������� Ȯ��
        foreach (var recipe in recipes)
        {
            if (MatchRecipe(filteredCards, recipe))
            {
                Debug.Log("������ ��ġ!");

                // Human ī�带 �θ𿡼� �и� (���� ����)
                if (triggerCard != null)
                    triggerCard.transform.SetParent(null);

                // ������ ī�� �ı�
                foreach (var card in filteredCards)
                    Destroy(card.gameObject);

                // ��� ī�� ����
                GameObject newCardObj = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                Card2D newCard = newCardObj.GetComponent<Card2D>();
                newCard.cardData = recipe.result;
                newCardObj.name = recipe.result.name;
                newCardObj.transform.SetParent(null);

                Debug.Log("�� ī�� ����: " + recipe.result.name);
            }
        }

        // ��ġ�ϴ� �����ǰ� ���� ���
        Debug.Log("��ġ�ϴ� ������ ����");
    }

    /// <summary>
    /// �־��� ī�� ����Ʈ�� Ư�� �����ǿ� ��Ȯ�� ��ġ�ϴ��� Ȯ���մϴ�.
    /// </summary>
    /// <param name="inputCards">���� ���� �õ� ���� ī���</param>
    /// <param name="recipe">���� ������</param>
    /// <returns>��ġ�ϸ� true, �ƴϸ� false</returns>
    private bool MatchRecipe(List<Card2D> inputCards, RecipeCardData recipe)
    {
        // ī�� ������ ������ ��ųʸ��� ����
        var inputDict = new Dictionary<CardData, int>();
        foreach (var card in inputCards)
        {
            if (inputDict.ContainsKey(card.cardData))
                inputDict[card.cardData]++;
            else
                inputDict[card.cardData] = 1;
        }

        // ������ �䱸���װ� ��
        foreach (var ingredient in recipe.ingredients)
        {
            var cardData = ingredient.ingredient;
            var requiredCount = ingredient.quantity;

            // �ش� ī�尡 ���ų� ���� ������ ��� ����
            if (!inputDict.TryGetValue(cardData, out int currentCount) || currentCount < requiredCount)
                return false;

            // ���� ����
            inputDict[cardData] -= requiredCount;

            // �� ������ ��ųʸ����� ����
            if (inputDict[cardData] == 0)
                inputDict.Remove(cardData);
        }

        // ������ ���� ī�尡 �� ������ ����
        return inputDict.Count == 0;
    }
}

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
        // �� �� ��� Card2D ������Ʈ�� ã�Ƽ� �迭�� ������
        Card2D[] allCards = FindObjectsByType<Card2D>(FindObjectsSortMode.None);
        List<Card2D> topCards = new List<Card2D>();

        // �θ� ���� ī�常 ���͸� -> ������ �ֻ��� ī��鸸 ����
        foreach (var card in allCards)
            if (card.transform.parent == null)
                topCards.Add(card);

        // �� �ֻ��� ī�庰�� ���� ������ �������� Ȯ�� �� ���� �õ�
        foreach (var topCard in topCards)
        {
            // �ڽ� ī�尡 ������ ������ �ƴϹǷ� �ǳʶ�
            if (topCard.transform.childCount == 0)
                continue;

            // ��ȿ�� ���� �������� �˻�, ����� ���ÿ� ���Ե� ī�� ����Ʈ ��ȯ
            if (IsValidCombinationStack(topCard, out List<Card2D> stackGroup))
                TryCombine(stackGroup); // ���� �õ�
        }
    }

    // ���� ������ ī�� �������� �˻��ϴ� �Լ�
    // ����: Human ī�尡 �� 1�� ���ԵǾ� �־�� ��
    private bool IsValidCombinationStack(Card2D topCard, out List<Card2D> stackGroup)
    {
        stackGroup = new List<Card2D>();
        int humanCount = 0;

        Transform current = topCard.transform;

        // ������ ������ �Ʒ��� ���󰡸� ī����� ����
        while (current != null)
        {
            Card2D card = current.GetComponent<Card2D>();

            // Card2D ������Ʈ�� ������ ����
            if (card == null)
                break;

            stackGroup.Add(card);

            // �ش� ī�尡 Human Ÿ������ �˻�
            if (card.IsCharacterOfType(card.cardData, CharacterType.Human))
                humanCount++;

            // �ڽ��� ������ ù ��° �ڽ����� �̵�, ������ null�� ���� ����
            current = current.childCount > 0 ? current.GetChild(0) : null;
        }

        // Human ī�尡 ��Ȯ�� 1���̰� �� ī�尡 ������ ���� �Ʒ�(������)�� ���� ���� true ��ȯ
        return humanCount == 1 && stackGroup[^1].IsCharacterOfType(stackGroup[^1].cardData, CharacterType.Human);
    }

    /// <summary>
    /// ī�� ����Ʈ�� ������ �õ��Ͽ� ���� �� ���ο� ī�带 �����մϴ�.
    /// </summary>
    /// <param name="cards">���տ� ���� ī�� ����Ʈ</param>
    public void TryCombine(List<Card2D> cards)
    {
        List<Card2D> filteredCards = new List<Card2D>(); // Human ī�带 ������ ���� ��� ī�� ����Ʈ
        Card2D triggerCard = null; // Human ī�带 ���� ����

        // ī�� �� Human ī��� Ʈ���� ���ҷ� �и�, �������� ���� ��� �߰�
        foreach (var card in cards)
        {
            if (card.IsCharacterOfType(card.cardData, CharacterType.Human))
                triggerCard = card;
            else
                filteredCards.Add(card);
        }

        // ��ϵ� ��� �����ǿ� ���Ͽ� ���� �������� �˻�
        foreach (var recipe in recipes)
        {
            if (MatchRecipe(filteredCards, recipe))
            {
                Debug.Log("������ ��ġ!");

                // Human ī�带 �θ𿡼� �и��� ���� ����
                if (triggerCard != null)
                    triggerCard.transform.SetParent(null);

                // ������ ī����� ��� �ı�
                foreach (var card in filteredCards)
                    Destroy(card.gameObject);

                // ���� ��� ī�� ���� �� �ʱ�ȭ
                GameObject newCardObj = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                Card2D newCard = newCardObj.GetComponent<Card2D>();
                newCard.cardData = recipe.result;
                newCardObj.name = recipe.result.name;
                newCardObj.transform.SetParent(null);

                Debug.Log("�� ī�� ����: " + recipe.result.name);
            }
        }

        // ��ġ�ϴ� �����ǰ� ������ �α� ���
        Debug.Log("��ġ�ϴ� ������ ����");
    }

    /// <summary>
    /// �־��� ī�� ����Ʈ�� Ư�� �����ǿ� ��Ȯ�� ��ġ�ϴ��� Ȯ���մϴ�.
    /// </summary>
    /// <param name="inputCards">���� ���� �õ� ���� ī�� ����Ʈ</param>
    /// <param name="recipe">���� ������ ������</param>
    /// <returns>�����ǿ� ��ġ�ϸ� true, �ƴϸ� false</returns>
    private bool MatchRecipe(List<Card2D> inputCards, RecipeCardData recipe)
    {
        // �Է� ī����� ī�� ������ ������ ����
        var inputDict = new Dictionary<CardData, int>();
        foreach (var card in inputCards)
        {
            if (inputDict.ContainsKey(card.cardData))
                inputDict[card.cardData]++;
            else
                inputDict[card.cardData] = 1;
        }

        // �������� �� ��Ằ �䱸 ������ ��
        foreach (var ingredient in recipe.ingredients)
        {
            var cardData = ingredient.ingredient;
            var requiredCount = ingredient.quantity;

            // �ش� ī�尡 ���ų� �䱸 �������� ������ ����
            if (!inputDict.TryGetValue(cardData, out int currentCount) || currentCount < requiredCount)
                return false;

            // ����� ���� ����
            inputDict[cardData] -= requiredCount;

            // ��뷮�� 0�̸� ��ųʸ����� ����
            if (inputDict[cardData] == 0)
                inputDict.Remove(cardData);
        }

        // ���� ī�尡 ������ �����ǿ� ����ġ -> ����
        return inputDict.Count == 0;
    }
}

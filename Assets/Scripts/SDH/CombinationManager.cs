using System.Collections.Generic;
using UnityEngine;

public class CombinationManager : MonoBehaviour
{
    public List<CardRecipe> recipes; // ���� ������ ������ ���
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
                newCard.cardData = recipe.resultCard;
                newCardObj.name = recipe.resultCard.name;

                // �ֻ����� ���� (�θ� ����)
                newCardObj.transform.SetParent(null);

                Debug.Log("�� ī�� ����: " + recipe.resultCard.name);
                return recipe.resultCard;
            }
        }

        Debug.Log("��ġ�ϴ� ������ ����");
        return null;
    }

    /// <summary>
    /// ī�� ����Ʈ�� �����ǿ� ��Ȯ�� ��ġ�ϴ��� Ȯ��
    /// </summary>
    private bool MatchRecipe(List<Card2D> inputCards, CardRecipe recipe)
    {
        var input = new List<CardData>();
        foreach (var c in inputCards)
            input.Add(c.cardData);

        var required = new List<CardData>(recipe.requiredCards);

        // required ��Ͽ� ���Ե� ī�尡 input�� �ִ��� �˻��ϰ� ����
        foreach (var data in required)
        {
            if (input.Contains(data))
                input.Remove(data);
            else
                return false; // �ʿ��� ī�尡 ������ ����
        }

        // ���� ī�尡 ����� ��Ȯ�� ��ġ
        return input.Count == 0;
    }
}

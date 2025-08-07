using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ���� �� ī�� ������ �����ϴ� �Ŵ��� Ŭ����
/// �ֻ��� ī�� ������ �˻��Ͽ� ��ȿ�� �����̸� �� ī��� ������
/// </summary>
public class CombinationManager : MonoBehaviour
{
    [SerializeField] private List<RecipeCardData> recipes; // ���� ������ ������ ���
    [SerializeField] private GameObject fieldCards;        // �ʵ忡 ���� ī����� �θ� ������Ʈ

    public static CombinationManager Instance { get; private set; }

    private void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // ���� �Ѿ�� ����
    }

    public void CheckCombination()
    {
        // �� �� �����ϴ� ��� Card2D ������Ʈ�� ã�Ƽ� �迭�� ������
        Card2D[] allCards = FindObjectsByType<Card2D>(FindObjectsSortMode.None);
        List<Card2D> topCards = new List<Card2D>(); // �ֻ��� ī���(������ �� �� ī��) ����� ����Ʈ

        // ��� ī�� �߿��� �θ� fieldCards�� ��츸 ��� topCards�� �߰�
        foreach (var card in allCards)
            if (card.transform.parent == fieldCards.transform)
                topCards.Add(card);

        // �� �ֻ��� ī�庰�� ������ ������ �������� �˻��ϰ�, ��ȿ�ϸ� ���� �õ�
        foreach (var topCard in topCards)
        {
            // �ڽ� ī�尡 ������ ���� ī���̹Ƿ� ���� �˻翡�� ����
            if (topCard.transform.childCount == 0)
                continue;

            // ��ȿ�� ���� �������� �˻� -> ���ǿ� ������ ������ ��ȯ
            if (IsValidCombinationStack(topCard, out List<Card2D> stackGroup))
                TryCombine(stackGroup); // ���� �õ�
        }
    }

    /// <summary>
    /// ���� ������ ī�� �������� �˻��ϴ� �Լ�
    /// ����: Human ī�尡 �� 1�� ���ԵǾ� ������, �ݵ�� ������ ������(�� �Ʒ�)�� ��ġ�ؾ� ��
    /// </summary>
    /// <param name="topCard">������ �ֻ��� ī��</param>
    /// <param name="stackGroup">���ÿ� ���Ե� ī����� ��ȯ</param>
    /// <returns>������ �����ϸ� true</returns>
    private bool IsValidCombinationStack(Card2D topCard, out List<Card2D> stackGroup)
    {
        stackGroup = new List<Card2D>();
        int humanCount = 0;

        Transform current = topCard.transform;

        // ������ ������ �Ʒ��� ��ȸ�ϸ� ī����� ����
        while (current != null)
        {
            Card2D card = current.GetComponent<Card2D>();

            // Card2D�� ������ ���� ��ȸ ����
            if (card == null)
                break;

            stackGroup.Add(card);

            // Human Ÿ������ Ȯ��
            if (card.IsCharacterOfType(card.cardData, CharacterType.Human))
                humanCount++;

            // ���� ī��� �̵� (�ڽ��� ������ ù ��° �ڽ�����, ������ null)
            //current = current.childCount > 0 ? current.GetChild(0) : null;
            if(card.childCards != null && card.childCards.Count > 0)
            {
                current = card.childCards[0].transform; // ù ��° �ڽ� ī��� �̵�
            }
            else
                current = null; // �� �̻� �ڽ��� ������ ��ȸ ����            
        }

        // ����� ������� ���� ���¸� Ȯ��
        Debug.Log($"[IsValidCombinationStack] ���� �˻�: {topCard.name}");
        Debug.Log($"- ���� ī�� ��: {stackGroup.Count}");
        Debug.Log($"- Human ī�� ����: {humanCount}");
        Debug.Log($"- ������ ī��: {stackGroup[^1].name}");

        // ������ ������ ī�尡 Human���� Ȯ��
        bool lastCardIsHuman = stackGroup[^1].IsCharacterOfType(stackGroup[^1].cardData, CharacterType.Human);
        Debug.Log($"- ������ ī�尡 Human�ΰ�? {lastCardIsHuman}");

        // ����: Human ī�尡 �� 1���̸�, ������ ������ ī�忩�� ��
        return humanCount == 1 && lastCardIsHuman;
    }

    /// <summary>
    /// ī�� ����Ʈ�� ������� ������ �õ��ϰ�, ���� �� �� ī�带 ������
    /// </summary>
    /// <param name="cards">���տ� ���� ī�� ����Ʈ</param>
    public void TryCombine(List<Card2D> cards)
    {
        List<Card2D> filteredCards = new List<Card2D>(); // Human ī�带 ������ ���� ��� ī���
        Card2D triggerCard = null; // Human ī�� ����� (������ Ʈ���� ����)

        // Human ī�带 ���� �����ϰ� ������ ī�常 ���� ������� �з�
        foreach (var card in cards)
        {
            if (card.IsCharacterOfType(card.cardData, CharacterType.Human))
                triggerCard = card;
            else
                filteredCards.Add(card);
        }

        // ��� �����Ǹ� ��ȸ�ϸ� ���� ������ �´��� Ȯ��
        foreach (var recipe in recipes)
        {
            if (MatchRecipe(filteredCards, recipe))
            {
                Debug.Log("������ ��ġ!");

                // Human ī�带 ���ÿ��� �и� (�ڽ� ���� ����)
                if (triggerCard != null)
                    triggerCard.transform.SetParent(null);

                // ���տ� ���� ī��(��� ī��)���� ��� �ı�
                foreach (var card in filteredCards)
                    CardManager.Instance.DestroyCard(card);

                if (triggerCard != null)
                {
                    // �� ī�� ���� ��ġ ��� (Human ī�� ���� �ణ �Ʒ�������)
                    SpriteRenderer triggerRenderer = triggerCard.GetComponent<SpriteRenderer>();
                    Vector3 spawnPosition = triggerCard.transform.position;
                    spawnPosition.y -= 0.2f;

                    // ���ο� ī�� ����
                    Card2D newCard = CardManager.Instance.SpawnCard(recipe.result, spawnPosition);
                    newCard.BringToFrontRecursive(newCard);
                    newCard.cardAnim.PlayFeedBack_ByName("BounceY");

                    // �� ī���� �θ� fieldCards�� ����
                    newCard.transform.SetParent(fieldCards.transform);

                    // newCard localPosition.z 0���� ����
                    Vector3 newLocalPos = newCard.transform.localPosition;
                    newLocalPos.z = 0f;
                    newCard.transform.localPosition = newLocalPos;

                    // triggerCard localPosition.z 0���� ����
                    Vector3 triggerLocalPos = triggerCard.transform.localPosition;
                    triggerLocalPos.z = 0f;
                    triggerCard.transform.localPosition = triggerLocalPos;

                    // ������ ���� ���� (Human ī�庸�� ���� ���̵���)
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
                            Debug.Log($"��ũ��Ʈ ���� �Ϸ�: {scriptName}");
                        }
                        else
                        {
                            Debug.LogError($"��ũ��Ʈ '{scriptName}' �� ã�� �� �����ϴ�. Ŭ�������� ��Ȯ���� Ȯ�����ּ���.");
                        }
                    }

                    Debug.Log($"newCard.cardData.cardName: {newCard.RuntimeData.cardName}");
                    Debug.Log("�� ī�� ����: " + recipe.result.name);
                }

                // Human ī���� ���׹̳� ���� ó��
                Human human = triggerCard.GetComponent<Human>();
                if (human != null)
                {
                    human.ConsumeStamina(1);
                }

                return; // ���� ���� �� �Լ� ����
            }
        }

        // ��ġ�ϴ� �����ǰ� ���� ��� �α� ���
        Debug.Log("��ġ�ϴ� ������ ����");
    }

    /// <summary>
    /// ���� ī�� ����Ʈ�� �־��� �����ǿ� ��Ȯ�� ��ġ�ϴ��� Ȯ��
    /// </summary>
    /// <param name="inputCards">���տ� ���� ī��� (Human ����)</param>
    /// <param name="recipe">�˻��� ������</param>
    /// <returns>��ġ�ϸ� true</returns>
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

        // ������ ���� ��
        foreach (var ingredient in recipe.ingredients)
        {
            var cardData = ingredient.ingredient;
            var requiredCount = ingredient.quantity;

            // �䱸�ϴ� ī�尡 ���ų�, ������ �����ϸ� ��ġ���� ����
            if (!inputDict.TryGetValue(cardData, out int currentCount) || currentCount < requiredCount)
                return false;

            // ����� ���� ��ųʸ����� ���� ����
            inputDict[cardData] -= requiredCount;

            if (inputDict[cardData] == 0)
                inputDict.Remove(cardData);
        }

        // ��� ��Ḧ ��Ȯ�� ����ߴ��� Ȯ��
        // ���� ī�尡 ������ ����ġ
        return inputDict.Count == 0;
    }
}

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    [Header("Card References")]
    [SerializeField] public Card2D cardPrefab;
    [SerializeField] public Transform cardParent;
    [SerializeField] public CardDatabase cardDatabase;

    private List<Card2D> fieldCards = new();
    private Dictionary<CardType, List<Card2D>> fieldCardsByType = new();

    public IReadOnlyList<Card2D> AllCardsOnField => fieldCards;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("[CardManager] ī�� �����ͺ��̽��� �ٽ� �ε��մϴ�.");
            cardDatabase.BuildTypeMap();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnCardById("071", new Vector3(0, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SpawnCardById("031", new Vector3(0, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SpawnCardById("051", new Vector3(0, 0, 0));
        }        
        if (Input.GetKeyDown(KeyCode.E))
        {
            foreach (var card in fieldCards)
            {
                Debug.Log($"Card: {card.name}, Type: {card.RuntimeData.cardType}, ID: {card.RuntimeData.cardId}");
            }
        }
    }


    // ==============ī�� ��ȯ �Լ�==============

    public Card2D SpawnCard(CardData data, Vector3 position)
    {
        if (data == null)
        {
            Debug.LogError("[CardManager] SpawnCard: ī�� �����Ͱ� null�Դϴ�.");
            return null;
        }

        Card2D newCard = Instantiate(cardPrefab, position, Quaternion.identity, cardParent);

        var runtimeData = data.Clone();
        var finalCard = AddCardScript(newCard.gameObject, runtimeData);
        finalCard.SetRuntimeData(runtimeData);
        finalCard.name = $"Card_{runtimeData.cardName}";

        Debug.Log($"[CardManager] ī�� ��ȯ: {finalCard.name} (ID: {runtimeData.cardId}) at {position}");
        RegisterCard(finalCard);
        return finalCard;
    }

    public Card2D SpawnCardById(string cardId, Vector3 position)
    {
        var data = cardDatabase.GetCardById(cardId);
        if (data == null)
        {
            Debug.LogError($"[CardManager] ī�� ID '{cardId}'�� �ش��ϴ� ī�尡 �����ϴ�.");
            return null;
        }
        return SpawnCard(data, position);
    }

    public Card2D SpawnCardByName(string cardName, Vector3 position)
    {
        var data = cardDatabase.GetCardByName(cardName);
        if (data == null)
        {
            Debug.LogError($"[CardManager] ī�� �̸� '{cardName}'�� �ش��ϴ� ī�尡 �����ϴ�.");
            return null;
        }
        return SpawnCard(data, position);
    }
    public void DestroyCard(Card2D card)
    {
        if (card == null)
        {
            Debug.LogWarning("[CardManager] DestroyCard: null ī���Դϴ�.");
            return;
        }
        Debug.Log($"[CardManager] Destroying card: {card.name}");
        card.DetachChildrenBeforeDestroy();
        UnregisterCard(card);       // ��� �� Ÿ�� ��ųʸ����� ����
        Destroy(card.gameObject);   // GameObject ����
    }

    public Card2D AddCardScript(GameObject obj, CardData data)
    {
        switch (data.cardType)
        {
            case CardType.Resource:
                break;
            case CardType.Food:
                Destroy(obj.GetComponent<Card2D>()); // EquipmentCard2D�� Card2D�� ����ϹǷ�, Card2D ������Ʈ ����
                var food = obj.AddComponent<FoodCard2D>();
                food.cardData = data;
                return food;

            case CardType.Equipment:
                Destroy(obj.GetComponent<Card2D>()); // EquipmentCard2D�� Card2D�� ����ϹǷ�, Card2D ������Ʈ ����
                var equip = obj.AddComponent<EquipmentCard2D>();
                equip.cardData = data;
                return equip;

            case CardType.Heal:
                break;
            case CardType.Furniture:
                break;
            case CardType.Character:
                if (data is HumanCardData humanData)
                {
                    AddHumanScript(obj, humanData);
                }
                else
                {
                    Debug.LogWarning($"[CardManager] Unsupported character type: {data.cardType}");
                }
                break;
            default:
                Debug.LogWarning($"[CardManager] Unknown card type: {data.cardType}");
                break;
        }

        return obj.GetComponent<Card2D>();
    }

    public void AddHumanScript(GameObject obj, CardData data)
    {
        Human human = obj.AddComponent<Human>();
        human.ChangeCharData(data as HumanCardData);
    }

    // ==============ī�� ��� / ����==============

    public void RegisterCard(Card2D card)
    {
        if (!fieldCards.Contains(card))
        {
            fieldCards.Add(card);
            AddToTypeDictionary(card);
        }
    }

    public void UnregisterCard(Card2D card)
    {
        if (fieldCards.Remove(card))
        {
            RemoveFromTypeDictionary(card);
        }
    }

    private void AddToTypeDictionary(Card2D card)
    {
        CardType type = card.cardData.cardType;

        if (!fieldCardsByType.ContainsKey(type))
        {
            fieldCardsByType[type] = new List<Card2D>();
        }

        fieldCardsByType[type].Add(card);
    }

    private void RemoveFromTypeDictionary(Card2D card)
    {
        CardType type = card.RuntimeData.cardType;

        if (fieldCardsByType.ContainsKey(type))
        {
            fieldCardsByType[type].Remove(card);

            if (fieldCardsByType[type].Count == 0)
                fieldCardsByType.Remove(type);
        }
    }

    public List<Card2D> GetCardsByType(CardType type)
    {
        return fieldCardsByType.TryGetValue(type, out var list) ? list : new List<Card2D>();
    }

    public List<Card2D> GetCharacterType(List<Card2D> cardList, CharacterType characterType)
    {
        List<Card2D> result = new();
        foreach (var card in cardList)
        {
            if (card.RuntimeData is CharacterCardData charData && charData.characterType == characterType)
            {
                result.Add(card);
            }
        }
        return result;
    }
}

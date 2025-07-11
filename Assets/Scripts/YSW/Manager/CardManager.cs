using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    [Header("Card References")]
    [SerializeField] private Card2D cardPrefab;
    [SerializeField] private Transform cardParent;
    [SerializeField] private CardDatabase cardDatabase;

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
            Debug.Log("[CardManager] 카드 데이터베이스를 다시 로드합니다.");
            cardDatabase.BuildTypeMap();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            SpawnCardById("002", new Vector3(0, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            foreach (var card in fieldCards)
            {
                Debug.Log($"Card: {card.name}, Type: {card.cardData.cardType}, ID: {card.cardData.cardId}");
            }
        }
    }


    // ==============카드 소환 함수==============

    public Card2D SpawnCard(CardData data, Vector3 position)
    {
        if (data == null)
        {
            Debug.LogError("[CardManager] SpawnCard: 카드 데이터가 null입니다.");
            return null;
        }

        Card2D newCard = Instantiate(cardPrefab, position, Quaternion.identity, cardParent);
        newCard.cardData = data;
        newCard.name = $"Card_{data.cardName}";

        Debug.Log($"[CardManager] 카드 소환: {newCard.name} (ID: {data.cardId}) at {position}");
        RegisterCard(newCard);
        return newCard;
    }

    public Card2D SpawnCardById(string cardId, Vector3 position)
    {
        var data = cardDatabase.GetCardById(cardId);
        if (data == null)
        {
            Debug.LogError($"[CardManager] 카드 ID '{cardId}'에 해당하는 카드가 없습니다.");
            return null;
        }
        return SpawnCard(data, position);
    }

    public Card2D SpawnCardByName(string cardName, Vector3 position)
    {
        var data = cardDatabase.GetCardByName(cardName);
        if (data == null)
        {
            Debug.LogError($"[CardManager] 카드 이름 '{cardName}'에 해당하는 카드가 없습니다.");
            return null;
        }
        return SpawnCard(data, position);
    }
    public void DestroyCard(Card2D card)
    {
        if (card == null)
        {
            Debug.LogWarning("[CardManager] DestroyCard: null 카드입니다.");
            return;
        }
        Debug.Log($"[CardManager] Destroying card: {card.name}");
        UnregisterCard(card);       // 목록 및 타입 딕셔너리에서 제거
        Destroy(card.gameObject);   // GameObject 제거
    }
    // ==============카드 등록 / 제거==============

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
        CardType type = card.cardData.cardType;

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
}

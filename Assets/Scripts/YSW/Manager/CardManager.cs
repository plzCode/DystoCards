using System.Collections.Generic;
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
            Debug.Log("[CardManager] 카드 데이터베이스를 다시 로드합니다.");
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
        if (Input.GetKeyDown(KeyCode.E))
        {
            foreach (var card in fieldCards)
            {
                Debug.Log($"Card: {card.name}, Type: {card.RuntimeData.cardType}, ID: {card.RuntimeData.cardId}");
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

        // 카드 데이터 Clone → 반드시 RuntimeData용으로 사용
        var runtimeData = data.Clone();
        newCard.SetRuntimeData(runtimeData);  // 여기서 이벤트 연결도 같이 함
        newCard.name = $"Card_{runtimeData.cardName}";

        AddCardScript(newCard.gameObject, runtimeData);  // 이젠 여기서 Human.Initialize() 해도 문제 없음

        Debug.Log($"[CardManager] 카드 소환: {newCard.name} (ID: {runtimeData.cardId}) at {position}");
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
        card.DetachChildrenBeforeDestroy();
        UnregisterCard(card);       // 목록 및 타입 딕셔너리에서 제거
        Destroy(card.gameObject);   // GameObject 제거
    }

    public void AddCardScript(GameObject obj, CardData data)
    {
        switch(data.cardType)
        {
            case CardType.Resource:                
                break;
            case CardType.Food:                
                break;
            case CardType.Equipment:
                obj.AddComponent<EquipmentCard2D>();
                obj.GetComponent<EquipmentCard2D>().cardData = data;
                Destroy(obj.GetComponent<Card2D>()); // EquipmentCard2D는 Card2D를 상속하므로, Card2D 컴포넌트 제거
                break;
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
    }

    public void AddHumanScript(GameObject obj, CardData data)
    {
        Human human = obj.AddComponent<Human>();
        human.ChangeCharData(data as HumanCardData);
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

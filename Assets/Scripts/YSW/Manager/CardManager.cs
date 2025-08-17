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
    public void Start()
    {
        TurnManager.Instance.RegisterPhaseAction(TurnPhase.ExploreEnd, () => OneDayElapse());
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

        var runtimeData = data.Clone();
        var finalCard = AddCardScript(newCard.gameObject, runtimeData);
        finalCard.cardData = data;
        finalCard.SetRuntimeData(runtimeData);
        finalCard.name = $"Card_{runtimeData.cardName}";

        Debug.Log($"[CardManager] 카드 소환: {finalCard.name} (ID: {runtimeData.cardId}) at {position}");
        RegisterCard(finalCard);
        return finalCard;
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
    public void DestroyCard(Card2D card, float delay = 0f)
    {
        card.isStackable = false; // 스택 가능 여부를 false로 설정하여 스택에서 제거
        card.DetachChildrenBeforeDestroy(); // 자식 오브젝트들을 분리

        if (card == null)
        {
            Debug.LogWarning("[CardManager] DestroyCard: null 카드입니다.");
            return;
        }
        Debug.Log($"[CardManager] Destroying card: {card.name}");
        card.DetachChildrenBeforeDestroy();
        UnregisterCard(card);       // 목록 및 타입 딕셔너리에서 제거

        if(delay > 0f)
        {
            var dissovle = card.GetComponent<Dissolve>();
            if (dissovle != null)
            {
                dissovle.StartCoroutine(dissovle.Vanish(true, true)); // Dissolve 컴포넌트가 있다면 서서히 제거
            }
            Destroy(card.gameObject, delay);
        }
        else
        {
            Destroy(card.gameObject);   // GameObject 제거
        }

    }

    public Card2D AddCardScript(GameObject obj, CardData data)
    {
        if (!obj.TryGetComponent<CardTileBarrier>(out _))
            obj.AddComponent<CardTileBarrier>();
        if (data.cardType == CardType.Facility && !obj.TryGetComponent<FacilityNoOverlap>(out _))
            obj.AddComponent<FacilityNoOverlap>();

        switch (data.cardType)
        {
            case CardType.Resource:
                break;
            case CardType.Food:
                Destroy(obj.GetComponent<Card2D>()); // EquipmentCard2D는 Card2D를 상속하므로, Card2D 컴포넌트 제거
                var food = obj.AddComponent<FoodCard2D>();
                food.cardData = data;
                return food;

            case CardType.Equipment:
                Destroy(obj.GetComponent<Card2D>()); // EquipmentCard2D는 Card2D를 상속하므로, Card2D 컴포넌트 제거
                var equip = obj.AddComponent<EquipmentCard2D>();
                equip.cardData = data;
                return equip;

            case CardType.Heal:
                Destroy(obj.GetComponent<Card2D>()); // EquipmentCard2D는 Card2D를 상속하므로, Card2D 컴포넌트 제거
                var heal = obj.AddComponent<HealCard2D>();
                heal.cardData = data;
                return heal;
            case CardType.Furniture:
                if (data.cardId == "055" || data.cardId == "052" || data.cardId == "056")
                {
                    obj.AddComponent<Card_Storage>();
                }
                else if (data.cardId == "051")
                {
                    obj.AddComponent<Card_Bed>();
                }
                break;
            case CardType.Character:
                if (data is HumanCardData humanData)
                {
                    AddHumanScript(obj, humanData);
                    Recorder.Instance.AddHuman(humanData.cardName, TurnManager.Instance.TurnCount);
                }
                else
                {
                    Debug.LogWarning($"[CardManager] Unsupported character type: {data.cardType}");
                }
                break;
            case CardType.Facility:
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
        human.AddComponent<FacilityParentWatcher>();
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

    public void OneDayElapse()
    {
        Debug.Log("CardManager : OnDayElapse Func");
        List<Card2D> humans = GetCharacterType(GetCardsByType(CardType.Character), CharacterType.Human);
        foreach(var human in humans)
        {
            Human _human = human.GetComponent<Human>();
            if (human.RuntimeData != null && human.RuntimeData is HumanCardData humanData && _human !=null) //굳이굳이 사람인지 3번이나 확인하는 중
            {
                // 하루가 지남에 따라 허기, 정신력 감소
                _human.ConsumeFood();                
                _human.TakeStress(1f); // 기준 정해야함
                _human.RecoverStamina(3f); // 기준 정해야함

                // 허기나 정신력이 0 이하로 떨어지면 사망 처리
                if (humanData.CurrentHunger <= 0 || humanData.CurrentMentalHealth <= 0)
                {
                    _human.Die();
                }
            }
        }

        TurnManager.Instance.MarkActionComplete();
    }    
}

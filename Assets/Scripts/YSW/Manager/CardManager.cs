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
        finalCard.cardData = data;
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
    public void DestroyCard(Card2D card, float delay = 0f)
    {
        card.isStackable = false; // ���� ���� ���θ� false�� �����Ͽ� ���ÿ��� ����
        card.DetachChildrenBeforeDestroy(); // �ڽ� ������Ʈ���� �и�

        if (card == null)
        {
            Debug.LogWarning("[CardManager] DestroyCard: null ī���Դϴ�.");
            return;
        }
        Debug.Log($"[CardManager] Destroying card: {card.name}");
        card.DetachChildrenBeforeDestroy();
        UnregisterCard(card);       // ��� �� Ÿ�� ��ųʸ����� ����

        if(delay > 0f)
        {
            var dissovle = card.GetComponent<Dissolve>();
            if (dissovle != null)
            {
                dissovle.StartCoroutine(dissovle.Vanish(true, true)); // Dissolve ������Ʈ�� �ִٸ� ������ ����
            }
            Destroy(card.gameObject, delay);
        }
        else
        {
            Destroy(card.gameObject);   // GameObject ����
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
                Destroy(obj.GetComponent<Card2D>()); // EquipmentCard2D�� Card2D�� ����ϹǷ�, Card2D ������Ʈ ����
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

    public void OneDayElapse()
    {
        Debug.Log("CardManager : OnDayElapse Func");
        List<Card2D> humans = GetCharacterType(GetCardsByType(CardType.Character), CharacterType.Human);
        foreach(var human in humans)
        {
            Human _human = human.GetComponent<Human>();
            if (human.RuntimeData != null && human.RuntimeData is HumanCardData humanData && _human !=null) //���̱��� ������� 3���̳� Ȯ���ϴ� ��
            {
                // �Ϸ簡 ������ ���� ���, ���ŷ� ����
                _human.ConsumeFood();                
                _human.TakeStress(1f); // ���� ���ؾ���
                _human.RecoverStamina(3f); // ���� ���ؾ���

                // ��⳪ ���ŷ��� 0 ���Ϸ� �������� ��� ó��
                if (humanData.CurrentHunger <= 0 || humanData.CurrentMentalHealth <= 0)
                {
                    _human.Die();
                }
            }
        }

        TurnManager.Instance.MarkActionComplete();
    }    
}

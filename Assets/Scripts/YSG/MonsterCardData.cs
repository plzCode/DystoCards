using UnityEngine;

[CreateAssetMenu(menuName = "Cards/11.MonsterCardData", order = 12)]
public class MonsterCardData : CharacterCardData
{
    [Header("Monster Attributes")]
    [SerializeField] private CardData[] drops;


    public CardData[] Drops
    {
        get => drops;
        set
        {
            if (drops != value)
            {
                drops=value;
                RaiseDataChanged();
            }
        }
    }
}
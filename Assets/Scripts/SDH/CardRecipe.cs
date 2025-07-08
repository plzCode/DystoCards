using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardRecipe")]
public class CardRecipe : ScriptableObject
{
    public List<CardData> requiredCards;
    public CardData resultCard;
}

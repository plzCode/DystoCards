using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/06.RecipeCard", order = 7)]
public class RecipeCardData : CardData
{
    public List<IngredientEntry> ingredients = new List<IngredientEntry>();
    public CardData result;
    public string scriptName;
}
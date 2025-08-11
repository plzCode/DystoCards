using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/06.RecipeCard", order = 7)]
public class RecipeCardData : CardData
{
    public List<IngredientEntry> ingredients = new List<IngredientEntry>();
    public CardData result;
    public string scriptName;

    public override CardData Clone()
    {
        RecipeCardData clone = ScriptableObject.CreateInstance<RecipeCardData>();

        // 부모 클래스 필드 복사
        clone.cardId = this.cardId;
        clone.cardName = this.cardName;
        clone.cardImage = this.cardImage;
        clone.cardType = this.cardType;
        clone.description = this.description;
        clone.size = this.size;

        clone.ingredients = this.ingredients;
        clone.result = this.result;
        clone.scriptName = this.scriptName;

        return clone;}
}
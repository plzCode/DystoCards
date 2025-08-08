using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/10.Tech", order = 10)]
public class TechCardData : CardData
{
    public List<IngredientEntry> ingredients = new List<IngredientEntry>();
    public int remainingTime;         // 기술 남은 시간 (턴 수)
    public int totalTime;          //  기술 총시간
    public TechType techType;

    public string techDescription;
    public bool unlocked = false;

    public RecipeCardData unlockRecipe; // 해금되는 레시피

    public override CardData Clone()
    {
        TechCardData clone = ScriptableObject.CreateInstance<TechCardData>();

        // 부모 클래스 필드 복사
        clone.cardId = this.cardId;
        clone.cardName = this.cardName;
        clone.cardImage = this.cardImage;
        clone.cardType = this.cardType;
        clone.description = this.description;
        clone.size = this.size;

        clone.totalTime = this.totalTime;
        clone.remainingTime = this.remainingTime;
        clone.unlocked = this.unlocked;
        clone.unlockRecipe = this.unlockRecipe;

        return clone;
    }
}

public enum TechType
{
    Tech,
}
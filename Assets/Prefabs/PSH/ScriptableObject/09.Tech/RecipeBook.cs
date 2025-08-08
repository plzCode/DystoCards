using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeBook : MonoBehaviour
{
    public static RecipeBook Instance { get; private set; }

    private List<RecipeCardData> allRecipes = new();
    private List<RecipeCardData> unlockedRecipes = new();

    [Header("Resources 폴더 내의 레시피 경로")]
    [SerializeField] private string recipeFolderPath = "Recipes";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAllRecipes();
    }

    /// <summary>
    /// Resources 폴더에서 모든 레시피를 불러와 초기 해금 여부를 판단
    /// </summary>
    public void LoadAllRecipes()
    {
        allRecipes = Resources.LoadAll<RecipeCardData>(recipeFolderPath).ToList();

        unlockedRecipes.Clear();
        foreach (var recipe in allRecipes)
        {
            if (recipe.unlockedByDefault)
                unlockedRecipes.Add(recipe);
        }

        Debug.Log($"[RecipeBook] 총 레시피 수: {allRecipes.Count}, 해금된 레시피 수: {unlockedRecipes.Count}");
    }

    /// <summary>
    /// 특정 레시피를 해금
    /// </summary>
    public void UnlockRecipe(RecipeCardData recipe)
    {
        if (recipe != null && !unlockedRecipes.Contains(recipe))
        {
            unlockedRecipes.Add(recipe);
            Debug.Log($"[RecipeBook] 레시피 해금됨: {recipe.cardName}");
        }
    }

    /// <summary>
    /// 해당 레시피가 해금 상태인지 확인
    /// </summary>
    public bool IsUnlocked(RecipeCardData recipe)
    {
        return unlockedRecipes.Contains(recipe);
    }

    /// <summary>
    /// 현재 해금된 모든 레시피 반환
    /// </summary>
    public List<RecipeCardData> GetUnlockedRecipes()
    {
        return unlockedRecipes;
    }

    /// <summary>
    /// 모든 레시피 반환 (디버그용)
    /// </summary>
    public List<RecipeCardData> GetAllRecipes()
    {
        return allRecipes;
    }
}

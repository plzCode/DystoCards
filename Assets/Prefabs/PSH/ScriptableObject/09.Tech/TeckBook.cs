using System.Collections.Generic;
using UnityEngine;

public class TechBook : MonoBehaviour
{
    public static TechBook Instance { get; private set; }

    [Header("기술 카드 리소스 폴더 경로")]
    [SerializeField] private string techFolderPath = "Cards/Tech";

    private List<TechCardData> allTechs = new();
    private List<TechCardData> unlockedTechs = new();

    public IReadOnlyList<TechCardData> GetAllTechs() => allTechs;
    public IReadOnlyList<TechCardData> GetUnlockedTechs() => unlockedTechs;
    public IReadOnlyList<TechCardData> GetLockedTechs() => allTechs.FindAll(t => !t.unlocked);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        LoadTechCardsFromResources();
    }

    private void LoadTechCardsFromResources()
    {
        allTechs.Clear();
        unlockedTechs.Clear();

        var loadedTechs = Resources.LoadAll<TechCardData>(techFolderPath);

        foreach (var tech in loadedTechs)
        {
            allTechs.Add(tech);
            if (tech.unlocked)
                unlockedTechs.Add(tech);
        }

        Debug.Log($"[TechBook] 기술 총 개수: {allTechs.Count}, 해금된 기술: {unlockedTechs.Count}");
    }

    public void UnlockTech(TechCardData tech)
    {
        if (!tech.unlocked)
        {
            tech.unlocked = true;
            if (!unlockedTechs.Contains(tech))
                unlockedTechs.Add(tech);

            Debug.Log($"[TechBook] 기술 해금됨: {tech.cardName}");
        }
    }

    public bool IsUnlocked(TechCardData tech)
    {
        return tech.unlocked;
    }
}

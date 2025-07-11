#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Database/Card Database")]
public class CardDatabase : ScriptableObject
{
    public List<CardData> allCards = new();
    
    // 카드 타입별로 분류
    public Dictionary<CardType, List<CardData>> typeToCardList = new();

#if UNITY_EDITOR
    [ContextMenu("Auto Populate From Project")]
    public void AutoPopulate()
    {
        allCards.Clear();

        string[] guids = AssetDatabase.FindAssets("t:CardData");
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            CardData card = AssetDatabase.LoadAssetAtPath<CardData>(path);
            if (card != null)
            {
                allCards.Add(card);
            }
        }
        BuildTypeMap();
        Debug.Log($"[CardDatabase] 자동 등록된 카드 수: {allCards.Count}");
        EditorUtility.SetDirty(this); // 변경사항 저장
    }
#endif

    public void BuildTypeMap()
    {
        typeToCardList.Clear();

        foreach (var card in allCards)
        {
            if (!typeToCardList.ContainsKey(card.cardType))
                typeToCardList[card.cardType] = new List<CardData>();

            typeToCardList[card.cardType].Add(card);
        }
    }
    public CardData GetCardById(string id)
    {
        return allCards.FirstOrDefault(card => card.cardId == id);
    }

    public CardData GetCardByName(string name)
    {
        return allCards.FirstOrDefault(card => card.cardName == name);
    }
}

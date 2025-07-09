using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public List<Card2D> humanCards = new List<Card2D>();
    public List<Card2D> monsterCards = new List<Card2D>();

    private void Start()
    {
        Card2D[] allCards = FindObjectsByType<Card2D>(FindObjectsSortMode.None);

        foreach (var card in allCards)
        {
            if (card.cardData is CharacterCardData characterCard)
            {
                switch (characterCard.characterType)
                {
                    case CharacterType.Human:
                        humanCards.Add(card);
                        break;

                    case CharacterType.Monster:
                        monsterCards.Add(card);
                        break;
                }
            }
        }
    }
}

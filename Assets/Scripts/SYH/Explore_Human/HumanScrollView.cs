using System.Collections.Generic;
using UnityEngine;
public class HumanScrollView : MonoBehaviour
{
    

    [SerializeField] private ExploreInfo exploreInfoUI;
    [SerializeField] private GameObject content;
 
    private List<HumanIcon> pooledIcons = new List<HumanIcon>();

    private void Awake()
    {
        
        foreach (Transform child in content.transform)
        {
            HumanIcon icon = child.GetComponent<HumanIcon>();
            if (icon != null)
            {
                icon.gameObject.SetActive(false);
                pooledIcons.Add(icon);
            }
        }
    }

    

    

    private void OnEnable()
    {
        List<Card2D> humanCards = ExploreManager.Instance.registedHumans;
        


        for (int i = 0; i < pooledIcons.Count; i++)
        {
            if (i < humanCards.Count)
            {
                pooledIcons[i].gameObject.SetActive(true);
                pooledIcons[i].Setup(humanCards[i].GetComponent<Human>(), exploreInfoUI, gameObject, humanCards[i]);
            }
            else
            {
                pooledIcons[i].gameObject.SetActive(false);
            }
        }
    }


}

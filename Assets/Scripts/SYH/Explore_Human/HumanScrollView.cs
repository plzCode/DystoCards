using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        List<HumanCardData> humans = ExploreManager.Instance.registedHumans;

        Debug.Log("온이네이블");
        for (int i = 0; i < pooledIcons.Count; i++)
        {
            if (i < humans.Count)
            {
                pooledIcons[i].gameObject.SetActive(true);
                pooledIcons[i].Setup(humans[i], exploreInfoUI, gameObject);
            }
            else
            {
                pooledIcons[i].gameObject.SetActive(false);
            }
        }
    }


}

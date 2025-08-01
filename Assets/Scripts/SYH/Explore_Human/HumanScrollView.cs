using System.Collections.Generic;
using UnityEngine;
public class HumanScrollView : MonoBehaviour
{
    

    [SerializeField] private ExploreInfo exploreInfoUI;
    [SerializeField] private GameObject content;
    [SerializeField] public HumanCardData exampleHuman1;
 
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

    

    public void AddHuman(HumanCardData data)
    {
        HumanIcon icon = null;

        // 비활성화된 아이콘 직접 탐색
        foreach (var i in pooledIcons)
        {
            if (!i.gameObject.activeSelf)
            {
                icon = i;
                break;
            }
        }

        if (icon != null)
        {            
            icon.Setup(data, exploreInfoUI, gameObject);
            icon.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("풀에 남은 아이콘이 없습니다.");
        }
    }

    private void OnEnable()
    {
        List<HumanCardData> humans = ExploreManager.Instance.registedHumans;

        
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

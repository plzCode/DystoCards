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

        // ��Ȱ��ȭ�� ������ ���� Ž��
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
            Debug.LogWarning("Ǯ�� ���� �������� �����ϴ�.");
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

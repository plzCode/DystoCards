using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpendLocationScroll : MonoBehaviour
{
    [SerializeField] GameObject OpendInfoPrefab;
    [SerializeField] GameObject Content;


    public void ShowOpendLocationList(List<LocationInfo> locations)
    {        
        for (int i = 0; i < locations.Count; i++)
        {
            var location = Instantiate(OpendInfoPrefab, Content.transform);
            location.GetComponent<OpenLocationInfo>().Set(locations[i]);
        }
        gameObject.SetActive(true);
    }
    
    void ClearOpendLocationList()
    {
        for (int i = Content.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(Content.transform.GetChild(i).gameObject);
        }
    }

    private void OnDisable()
    {
        ClearOpendLocationList();
    }

}

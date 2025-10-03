using System.Collections.Generic;
using UnityEngine;

public class ExploringScrollView : MonoBehaviour
{

    [SerializeField] private GameObject content;

    [SerializeField] private List<ExploringInfo> pooledIcons = new List<ExploringInfo>();


    private void Awake()
    {
        // content 자식 중 ExploringInfo 가진 객체 전부 찾아서 pooledIcons에 넣기
        pooledIcons.Clear();

        for (int i = 0; i < content.transform.childCount; i++)
        {
            var child = content.transform.GetChild(i);
            var info = child.GetComponent<ExploringInfo>();
            if (info != null)
            {
                pooledIcons.Add(info);
                child.gameObject.SetActive(false); // 초기에는 모두 비활성화
            }
        }

        ExploreManager.Instance.OnExploreAdded += AddExploring;
        ExploreManager.Instance.OnExploreCompleted += RemoveExploring;
    }



    public void AddExploring(ExplorationData data)
    {
        ExploringInfo targetInfo = null;

        // 비활성화된 아이콘 찾아서 재활용
        foreach (var icon in pooledIcons)
        {
            if (!icon.gameObject.activeSelf)
            {
                targetInfo = icon;
                break;
            }
        }

        if (targetInfo == null)
        {
            Debug.LogWarning("풀에 남은 아이콘이 없습니다! 새 아이콘을 추가하는 로직 필요.");
            return;
        }

        // 데이터 세팅 후 활성화
        targetInfo.SetData(data);
        targetInfo.gameObject.SetActive(true);
        

    }

    // 탐험 완료된 아이콘 비활성화 또는 반환하는 함수 구현
    private void RemoveExploring(ExplorationData data)
    {
        // registeredExplorations와 탐색중인 UI가 1:1 대응된다고 가정
        foreach (var icon in pooledIcons)
        {
            if (icon.Data == data)
            {
                icon.gameObject.SetActive(false);
                break;
            }
        }
    }

    private void OnEnable()
    {
        //ExploreManager.Instance.OnExploreAdded += AddExploring;
        //ExploreManager.Instance.OnExploreCompleted += RemoveExploring;
    }

    private void OnDestroy()
    {
        ExploreManager.Instance.OnExploreAdded -= AddExploring;
        ExploreManager.Instance.OnExploreCompleted -= RemoveExploring;
    }    

}

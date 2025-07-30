using System.Collections.Generic;
using UnityEngine;

public class ExploringScrollView : MonoBehaviour
{

    [SerializeField] private GameObject content;

    [SerializeField] private List<ExploringInfo> pooledIcons = new List<ExploringInfo>();


    private void Awake()
    {
        // content �ڽ� �� ExploringInfo ���� ��ü ���� ã�Ƽ� pooledIcons�� �ֱ�
        pooledIcons.Clear();

        for (int i = 0; i < content.transform.childCount; i++)
        {
            var child = content.transform.GetChild(i);
            var info = child.GetComponent<ExploringInfo>();
            if (info != null)
            {
                pooledIcons.Add(info);
                child.gameObject.SetActive(false); // �ʱ⿡�� ��� ��Ȱ��ȭ
            }
        }

        ExploreManager.Instance.OnExploreAdded += AddExploring;
        ExploreManager.Instance.OnExploreCompleted += RemoveExploring;
    }



    public void AddExploring(ExplorationData data)
    {
        ExploringInfo targetInfo = null;

        // ��Ȱ��ȭ�� ������ ã�Ƽ� ��Ȱ��
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
            Debug.LogWarning("Ǯ�� ���� �������� �����ϴ�! �� �������� �߰��ϴ� ���� �ʿ�.");
            return;
        }

        // ������ ���� �� Ȱ��ȭ
        targetInfo.SetData(data);
        targetInfo.gameObject.SetActive(true);
        

    }

    // Ž�� �Ϸ�� ������ ��Ȱ��ȭ �Ǵ� ��ȯ�ϴ� �Լ� ����
    private void RemoveExploring(ExplorationData data)
    {
        // registeredExplorations�� Ž������ UI�� 1:1 �����ȴٰ� ����
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

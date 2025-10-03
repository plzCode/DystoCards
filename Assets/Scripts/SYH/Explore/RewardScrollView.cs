using System.Collections.Generic;
using UnityEngine;

public class RewardScrollView : MonoBehaviour
{

    [SerializeField] private GameObject content;

    [SerializeField] private List<RewardSlotUI> pooledSlots = new List<RewardSlotUI>();

    public void Init()
    {
        pooledSlots.Clear();

        for (int i = 0; i < content.transform.childCount; i++)
        {
            var child = content.transform.GetChild(i);
            var slot = child.GetComponent<RewardSlotUI>();
            if (slot != null)
            {
                slot.Init();
                pooledSlots.Add(slot);
                child.gameObject.SetActive(false);
            }
        }

        ExploreManager.Instance.OnExploreCompleted += ShowRewards;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        ExploreManager.Instance.OnExploreCompleted -= ShowRewards;

    }

    private void ShowRewards(ExplorationData data)
    {
        gameObject.SetActive(true);
        Debug.Log("보상 보이기");
        // 보상 리스트 가져오기
        var rewards = ExploreManager.Instance.GetRewards(data.location, data.human.humanData);       

        RewardSlotUI targetInfo = null;

        // 비활성화된 아이콘 찾아서 재활용
        foreach (var icon in pooledSlots)
        {
            if (!icon.gameObject.activeSelf)
            {
                targetInfo = icon;
                break;
            }
        }

        targetInfo.Set(rewards,data.location.locationImage,data.human.humanData.cardImage);
        targetInfo.gameObject.SetActive(true);

    }

    public void Clear()
    {
        foreach (var slot in pooledSlots)
        {
            slot.Clear();
            slot.gameObject.SetActive(false);
        }

        gameObject.SetActive(false); // 보상창 자체도 숨기기
    }

}

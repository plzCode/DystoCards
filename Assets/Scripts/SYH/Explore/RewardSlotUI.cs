using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardSlotUI : MonoBehaviour
{
    [SerializeField] private GameObject content;

    [SerializeField] private GameObject rewardPrefab; // 카드 항목 프리팹 (Image + Text)

    [SerializeField] private Image humanImage;

    [SerializeField] private Image locationImage;

    public List<GameObject> pooledCardItems = new List<GameObject>();

    public void Init()
    {
        // 초기 풀링 세팅 (content의 자식으로 이미 존재하는 카드 항목들)
        pooledCardItems.Clear();
        for (int i = 0; i < content.transform.childCount; i++)
        {
            var child = content.transform.GetChild(i).gameObject;
            pooledCardItems.Add(child);
            child.SetActive(false);
        }

    }

    public void Set(List<RewardInfo> rewards,Sprite locationSprite,Sprite humanSprite)
    {
        humanImage.sprite = humanSprite;
        locationImage.sprite = locationSprite;

        // 기존 카드 항목 모두 비활성화
        foreach (var item in pooledCardItems)
            item.SetActive(false);


        for (int i = 0; i < rewards.Count; i++)
        {
            GameObject item = null;

            foreach (var slot in pooledCardItems)
            {
                if (!slot.gameObject.activeSelf)
                {
                    item = slot;
                    break;
                }

            }


            var image = item.GetComponentInChildren<Image>();
            var text = item.GetComponentInChildren<TextMeshProUGUI>();

            image.sprite = rewards[i].card.cardImage;
            text.text = "x " + rewards[i].quantity.ToString();
            Debug.Log($" {rewards[i].card.name} 을 {rewards[i].quantity.ToString()} 개 획득");

            item.SetActive(true);
        }
    }

    public void Clear()
    {
        foreach (var item in pooledCardItems)
        {
            item.SetActive(false);
        }
    }
}
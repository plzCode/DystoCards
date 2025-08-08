using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardSlotUI : MonoBehaviour
{
    [SerializeField] private GameObject content;

    [SerializeField] private GameObject rewardPrefab; // ī�� �׸� ������ (Image + Text)

    [SerializeField] private Image humanImage;

    [SerializeField] private Image locationImage;

    public List<GameObject> pooledCardItems = new List<GameObject>();

    public void Init()
    {
        // �ʱ� Ǯ�� ���� (content�� �ڽ����� �̹� �����ϴ� ī�� �׸��)
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

        // ���� ī�� �׸� ��� ��Ȱ��ȭ
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
            Debug.Log($" {rewards[i].card.name} �� {rewards[i].quantity.ToString()} �� ȹ��");

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
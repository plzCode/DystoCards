using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;

public class CardInfoUI : MonoBehaviour
{
    public Image char_Image;
    public TextMeshProUGUI char_Name;
    public Transform char_Equipment;
    public Transform char_Stat_Icon;
    public HumanCardData charData;
    public GameObject stat;
    public StatPanelManager statPanelManager;
    public Card2D card2D;
    

    public void Initialize(GameObject charCard)
    {
        charData = charCard.GetComponent<Character>().charData as HumanCardData;
        Human character = charCard.GetComponent<Human>();
        Card2D card2D = charCard.GetComponent<Card2D>();
        if (charData == null) return;
        // 캐릭터 이미지 설정
        if (char_Image != null && charData.cardImage != null)
        {
            char_Image.sprite = charData.cardImage;
        }
        // 캐릭터 이름 설정
        if (char_Name != null)
        {
            char_Name.text = charData.cardName;
        }
        // 장비 아이템 설정
        if (char_Equipment != null)        {
            foreach(Transform child in char_Equipment)
            {                
                Destroy(child.gameObject);
            }
            foreach (var item in character.equipmentSlotList)
            {
                GameObject equipmentItem = new GameObject(item.equipment.cardName);
                equipmentItem.transform.SetParent(char_Equipment);
                equipmentItem.AddComponent<Image>().sprite = item.equipment.cardImage;
                // 추가적인 UI 설정 필요
            }
        }
        // 슬롯 설정
        if (char_Stat_Icon != null)
        {
            
            /*foreach (var slot in card2D.GetStatDictionaryFromCardData(charData))
            {
                *//*GameObject slotItem = Instantiate(stat);
                slotItem.name = slot.Key;
                slotItem.transform.SetParent(char_Stat_Icon);*//*

                // 추가적인 UI 설정 필요
            }*/
                statPanelManager.DisplayStats(charData, character, card2D);
        }

        
    }

}

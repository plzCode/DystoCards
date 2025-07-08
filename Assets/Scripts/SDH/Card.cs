using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public CardData data;           // 연결할 카드 데이터 (ScriptableObject)
    //public Image cardImage;         // 카드에 표시할 이미지
    private Text cardNameText;       // 카드에 표시할 이름 (선택 사항)

    void Start()
    {
        UpdateVisuals();
    }

    // 카드 데이터에 따라 UI를 업데이트
    public void UpdateVisuals()
    {
        if (data != null)
        {
            //if (cardImage != null)
                //cardImage.sprite = data.cardSprite;

            if (cardNameText != null)
                cardNameText.text = data.cardName;
        }
    }
}

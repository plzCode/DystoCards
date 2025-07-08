using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public CardData data;           // ������ ī�� ������ (ScriptableObject)
    //public Image cardImage;         // ī�忡 ǥ���� �̹���
    private Text cardNameText;       // ī�忡 ǥ���� �̸� (���� ����)

    void Start()
    {
        UpdateVisuals();
    }

    // ī�� �����Ϳ� ���� UI�� ������Ʈ
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

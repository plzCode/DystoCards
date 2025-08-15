using Unity.VisualScripting;
using UnityEngine;

public class MouseInput : MonoBehaviour
{
    public LayerMask interactableLayerMask;
    public LayerMask cardLayer;
    public bool isOverInteractable = false;
    private Card2D selectedCard = null;

    private void Update()
    {
        CheckMouseOver();
        HandleMouseInput();        
    }

    private void CheckMouseOver()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, interactableLayerMask);

        if (hit.collider != null)
        {
            if (!isOverInteractable)
            {
                isOverInteractable = true;
                UIManager.Instance.SetInteractCursor();
            }
        }
        else
        {
            if (isOverInteractable)
            {
                isOverInteractable = false;
                UIManager.Instance.SetDefaultCursor();
            }
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Card2D card = RaycastForCard();
            if (card != null)
            {
                selectedCard = card;
                Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                selectedCard.StartDragging(mouseWorld);
                AudioManager.Instance.PlaySFX("Click_3");
            }
        }
        else if (Input.GetMouseButton(0) && selectedCard != null)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            selectedCard.Dragging(new Vector3(mouseWorld.x, mouseWorld.y, 0));
        }
        else if (Input.GetMouseButtonUp(0) && selectedCard != null)
        {
            selectedCard.EndDragging();
            selectedCard = null;
            AudioManager.Instance.PlaySFX("Click_3");
        }

        // ��Ŭ�� ī�� ����
        if (Input.GetMouseButtonDown(1))
        {
            /*Card2D card = RaycastForCard();
            if (card != null)
            {
                Debug.Log($"[RightClick] {card.name} ��Ŭ����!");
                CardManager.Instance.DestroyCard(card);
            }*/
            Card2D card = RaycastForCard();
            if (card != null)
            {
                switch (card.RuntimeData.cardType)
                {
                    case CardType.Character:
                        HumanCardData humanData = card.RuntimeData as HumanCardData;
                        if (humanData != null)
                        {
                            Debug.Log($"[RightClick] {card.name} ��Ŭ����! (�ΰ� ī��)");

                            // �г� ��������
                            var infoPanel = UIManager.Instance.cardInfoPanel;
                            var canvasGroup = infoPanel.GetComponent<CanvasGroup>();

                            // �г� ���̱�
                            UIManager.Instance.TogglePanel(infoPanel);
                            AudioManager.Instance.PlaySFX("Book_1");

                            // ī�� ���� �ʱ�ȭ
                            infoPanel.GetComponent<CardInfoUI>().Initialize(card.gameObject);
                        }
                        break;
                    default:
                        Debug.Log($"[RightClick] {card.name} ��Ŭ����! (��Ÿ ī�� Ÿ��: {card.RuntimeData.cardType})");
                        break;
                }
            }
        }

        if(Input.GetMouseButtonDown(2)) // �� Ŭ��
        {
            Card2D card = RaycastForCard();
            if (card != null)
            {
                Debug.Log($"[MiddleClick] {card.name} �� Ŭ����!");
                //float delay = card.cardAnim.PlayFeedBack_ByName("DustParticle");
                CardManager.Instance.DestroyCard(card, 1f);

            }
        }
    }

    private Card2D RaycastForCard()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, cardLayer);

        if (hit.collider != null)
        {
            return hit.collider.GetComponent<Card2D>();
        }

        return null;
    }

    //���̾� ����ũ ��ȯ �Լ�
    public void SetInteractionLayers(LayerMask interactMask, LayerMask cardMask)
    {
        interactableLayerMask = interactMask;
        cardLayer = cardMask;
    }

}

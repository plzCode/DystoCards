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
        }

        // 우클릭 카드 삭제
        if (Input.GetMouseButtonDown(1))
        {
            /*Card2D card = RaycastForCard();
            if (card != null)
            {
                Debug.Log($"[RightClick] {card.name} 우클릭됨!");
                CardManager.Instance.DestroyCard(card);
            }*/
            Card2D card = RaycastForCard();
            if (card != null)
            {
                switch (card.cardData.cardType)
                {
                    case CardType.Character:
                        HumanCardData humanData = card.cardData as HumanCardData;
                        if (humanData != null)
                        {
                            Debug.Log($"[RightClick] {card.name} 우클릭됨! (인간 카드)");

                            // 패널 가져오기
                            var infoPanel = UIManager.Instance.cardInfoPanel;
                            var canvasGroup = infoPanel.GetComponent<CanvasGroup>();

                            // 패널 보이기
                            UIManager.Instance.TogglePanel(infoPanel);

                            // 카드 정보 초기화
                            infoPanel.GetComponent<CardInfoUI>().Initialize(card.gameObject);
                        }
                        break;
                    default:
                        Debug.Log($"[RightClick] {card.name} 우클릭됨! (기타 카드 타입: {card.cardData.cardType})");
                        break;
                }
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

}

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
            Card2D card = RaycastForCard();
            if (card != null)
            {
                Debug.Log($"[RightClick] {card.name} 우클릭됨!");
                CardManager.Instance.DestroyCard(card);
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

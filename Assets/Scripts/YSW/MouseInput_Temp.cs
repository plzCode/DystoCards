using UnityEngine;

public class MouseInput_Temp : MonoBehaviour
{
    public LayerMask interactableLayerMask;
    public bool isOverInteractable = false;

    private void Update()
    {
        CheckMouseOver();

        if (Input.GetMouseButtonDown(1)) // 1 = 우클릭
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                var card = hit.collider.GetComponent<Card2D>();
                if (card != null)
                {
                    Debug.Log($"[RightClick] {card.name} 우클릭됨!");
                    CardManager.Instance.DestroyCard(card);
                }
            }
        }        
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

}

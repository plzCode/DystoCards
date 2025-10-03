using UnityEngine;

public class MinimapController : MonoBehaviour
{

    public RectTransform minimapPanel;   // 미니맵 UI 패널
    public RectTransform mapImage;       // 이동 대상 이미지

    [Header("줌 설정")]
    public float zoomSpeed = 0.1f;
    public float minZoom = 0.5f;
    public float maxZoom = 3f;

    private bool isDragging = false;
    public bool isDraggingIcon = false; // 아이콘 클릭 상태
    private Vector2 lastMousePos;

    void Update()
    {
        if (!Application.isFocused || isDraggingIcon)
        {
            isDragging = false;
            return;
        }

        Vector2 localMousePos;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapPanel, Input.mousePosition, null, out localMousePos))
            return;

        Vector2 panelSize = minimapPanel.rect.size;
        Vector2 halfSize = panelSize * 0.5f;

        // 줌 처리
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0)
        {
            float oldScale = mapImage.localScale.x;
            float newScale = Mathf.Clamp(oldScale + scroll * zoomSpeed, minZoom, maxZoom);

            if (!Mathf.Approximately(newScale, oldScale))
            {
                Vector2 pivotToMouse = localMousePos - mapImage.anchoredPosition;
                float scaleRatio = newScale / oldScale;
                Vector2 newAnchoredPos = mapImage.anchoredPosition - pivotToMouse * (scaleRatio - 1f);

                mapImage.localScale = new Vector3(newScale, newScale, 1f);
                mapImage.anchoredPosition = newAnchoredPos;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (localMousePos.x >= -halfSize.x && localMousePos.x <= halfSize.x &&
                localMousePos.y >= -halfSize.y && localMousePos.y <= halfSize.y)
            {
                isDragging = true;
                lastMousePos = localMousePos;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        Vector2 nextPos = mapImage.anchoredPosition;

        if (isDragging)
        {
            Vector2 delta = localMousePos - lastMousePos;
            nextPos += delta;
            lastMousePos = localMousePos;
        }

        Vector2 mapSize = mapImage.rect.size * mapImage.localScale;
        Vector2 limit = (mapSize - panelSize) * 0.5f;

        nextPos.x = Mathf.Clamp(nextPos.x, -limit.x, limit.x);
        nextPos.y = Mathf.Clamp(nextPos.y, -limit.y, limit.y);

        mapImage.anchoredPosition = nextPos;
    }

    public void CloseMiniMap()
    {
        gameObject.SetActive(false);
        isDragging = false;
        isDraggingIcon = false;
    }

}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MinimapIcon : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public MinimapController minimapController;    
    public LocationInfo locationInfo;
    public ExploreInfo exploreInfoUI;
    public Button iconButton; 

    private Vector3 originalScale;
    private Vector3 targetScale;
    private float scaleSpeed = 5f;
    private bool isHovered = false;

    void Start()
    {
        if (minimapController == null)
            minimapController = GetComponentInParent<MinimapController>();

        originalScale = transform.localScale;
        targetScale = originalScale;

        iconButton.onClick.AddListener(() => {
            exploreInfoUI.SetExploreInfo(locationInfo);
            exploreInfoUI.gameObject.SetActive(true);
        });
    }

    void Update()
    {
        Vector3 currentScale = transform.localScale;
        transform.localScale = Vector3.Lerp(currentScale, targetScale, Time.deltaTime * scaleSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        targetScale = originalScale * 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        targetScale = originalScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        minimapController.isDraggingIcon = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        minimapController.isDraggingIcon = false;
    }

    void OnDisable()
    {
        if (minimapController != null)
            minimapController.isDraggingIcon = false;

        // 비활성화 시 크기 초기화
        transform.localScale = originalScale;
        targetScale = originalScale;
        isHovered = false;
    }

    
}

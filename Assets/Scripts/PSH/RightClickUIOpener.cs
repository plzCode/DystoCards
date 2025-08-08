using UnityEngine;
using UnityEngine.UI;

public class RightClickUIOpener : MonoBehaviour
{
    public GameObject uiPanel;  // UI 창 (비활성화 상태여야 함)
    public LayerMask interactableLayer; // 오브젝트 레이어

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (uiPanel == null)
        {
            // 비활성화된 상태 포함 전체에서 찾기
            Canvas[] canvases = Resources.FindObjectsOfTypeAll<Canvas>();
            foreach (Canvas canvas in canvases)
            {
                if (canvas.gameObject.CompareTag("TechUI"))
                {
                    uiPanel = canvas.gameObject;
                    break;
                }
            }
        }



        uiPanel.SetActive(false); // 시작 시 비활성화
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // 우클릭
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, interactableLayer);

            if (hit.collider != null)
            {
                Debug.Log($"우클릭 대상: {hit.collider.name}");
                OpenUI(hit.collider.gameObject);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (uiPanel.activeSelf)
            {
                CloseUI();
            }
        }
    }

    void OpenUI(GameObject target)
    {
        uiPanel.SetActive(true);

        // 선택한 오브젝트 위치에 따라 UI를 옮기고 싶을 경우:
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.transform.position);
        uiPanel.transform.position = screenPos;
    }

    void CloseUI()
    {
        uiPanel.SetActive(false);
    }
}

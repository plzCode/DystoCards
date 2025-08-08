using UnityEngine;
using UnityEngine.UI;

public class RightClickUIOpener : MonoBehaviour
{
    public GameObject uiPanel;  // UI â (��Ȱ��ȭ ���¿��� ��)
    public LayerMask interactableLayer; // ������Ʈ ���̾�

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (uiPanel == null)
        {
            // ��Ȱ��ȭ�� ���� ���� ��ü���� ã��
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



        uiPanel.SetActive(false); // ���� �� ��Ȱ��ȭ
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // ��Ŭ��
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, interactableLayer);

            if (hit.collider != null)
            {
                Debug.Log($"��Ŭ�� ���: {hit.collider.name}");
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

        // ������ ������Ʈ ��ġ�� ���� UI�� �ű�� ���� ���:
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.transform.position);
        uiPanel.transform.position = screenPos;
    }

    void CloseUI()
    {
        uiPanel.SetActive(false);
    }
}

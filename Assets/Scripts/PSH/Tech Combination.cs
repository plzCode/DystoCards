using UnityEngine;
using UnityEngine.UIElements;

public class TechCombination : MonoBehaviour
{
    public LayerMask interactableLayer; // ������Ʈ ���̾�

    public GameObject Tech; // ���� ī�� ������Ʈ
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
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
                Destroy(gameObject); //���� ������Ʈ ���� 
                Instantiate(Tech, hit.collider.transform.position, Quaternion.identity); // ������ ��ũī�� ����

            }
        }
    }
}

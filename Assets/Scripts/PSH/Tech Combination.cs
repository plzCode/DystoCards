using UnityEngine;
using UnityEngine.UIElements;

public class TechCombination : MonoBehaviour
{
    public LayerMask interactableLayer; // 오브젝트 레이어

    public GameObject Tech; // 연구 카드 오브젝트
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
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
                Destroy(gameObject); //현재 오브젝트 제거 
                Instantiate(Tech, hit.collider.transform.position, Quaternion.identity); // 선택한 테크카드 생성

            }
        }
    }
}

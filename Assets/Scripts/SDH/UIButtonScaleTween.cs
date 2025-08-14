using UnityEngine;
using UnityEngine.EventSystems;

class UIButtonScaleEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float scaleFactor = 1.2f; // ��ư�� Ŀ�� ���� (1.0 = ���� ũ��, 1.1 = 10% Ŀ��)
    [SerializeField] private float scaleSpeed = 5f; // ũ�� ���� �ӵ�

    private Vector3 originalScale; // ��ư ���� ũ�� ���� ����
    private bool isHovering = false; // ���콺�� ��ư ���� �ִ��� ����

    private void Start()
    {
        // ���� �� ���� ���� �������� ���� ũ��� ����
        originalScale = transform.localScale;
    }

    private void Update()
    {
        // ���콺�� ��ư ���� ������ ũ�⸦ scaleFactor��� Ȯ��, �ƴϸ� ���� ũ��� ����
        Vector3 target = isHovering ? originalScale * scaleFactor : originalScale;

        // ���� ũ�⿡�� ��ǥ ũ��� �ε巴�� �����Ͽ� ����
        transform.localScale = Vector3.Lerp(transform.localScale, target, Time.deltaTime * scaleSpeed);
    }

    // ���콺 �����Ͱ� ��ư ���� ������ �� ȣ��Ǵ� �̺�Ʈ
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true; // ũ�� Ȯ�� ����
    }

    // ���콺 �����Ͱ� ��ư���� ������ �� ȣ��Ǵ� �̺�Ʈ
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false; // ũ�� ������� ��� ����
    }
}

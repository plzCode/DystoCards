using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TechTipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject techtipPanel; // 설명창 오브젝트
    public Button button;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable == false)
            techtipPanel.SetActive(false);
        else
            techtipPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        techtipPanel.SetActive(false);
    }
}
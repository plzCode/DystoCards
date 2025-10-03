using UnityEngine;

public class TechUI : MonoBehaviour
{
    public GameObject targetUI; // 비활성화할 UI 오브젝트

    public void Close()
    {
        if (targetUI != null)
        {
            targetUI.SetActive(false);
        }
    }

    public void Open()
    {
        if(targetUI !=null)
        {
            targetUI.SetActive(true);
        }
    }
}

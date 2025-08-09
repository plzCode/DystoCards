using UnityEngine;

public class TechUI : MonoBehaviour
{
    public GameObject targetUI; // ��Ȱ��ȭ�� UI ������Ʈ

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

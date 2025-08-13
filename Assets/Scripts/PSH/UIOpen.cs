using UnityEngine;
using UnityEngine.UI;

public class UIOpen : MonoBehaviour
{


    [SerializeField] private GameObject targetPanel; // �Ѱ� ���� UI �г�

    public void OpenPanel()
    {
        if (targetPanel != null)
            targetPanel.SetActive(true);
    }

}

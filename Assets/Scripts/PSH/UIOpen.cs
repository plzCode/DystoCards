using UnityEngine;
using UnityEngine.UI;

public class UIOpen : MonoBehaviour
{


    [SerializeField] private GameObject targetPanel; // 켜고 싶은 UI 패널

    public void OpenPanel()
    {
        if (targetPanel != null)
            targetPanel.SetActive(true);
    }

}

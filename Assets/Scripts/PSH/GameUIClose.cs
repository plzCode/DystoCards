using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIClose : MonoBehaviour
{

    [SerializeField] private GameObject targetPanel; // 켜고 싶은 UI 패널

    public void OpenUI()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (targetPanel != null)
                targetPanel.SetActive(true);
        }
    }

    public void OnReStartButton()
    {
        if (targetPanel != null)
            targetPanel.SetActive(false);
    }

    public void OnTitleButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnExitButton()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // �����Ϳ����� ���� ����
#endif
    }
}

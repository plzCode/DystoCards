using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string startSceneName = "Integration 6"; 

    public void OnStartButton()
    {
        SceneManager.LoadScene("Integration 6");
    }

    public void OnExitButton()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // �����Ϳ����� ���� ����
#endif
    }
}

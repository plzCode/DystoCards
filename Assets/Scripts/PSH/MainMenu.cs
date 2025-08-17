using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string startSceneName = "Integration 4"; 

    public void OnStartButton()
    {
        if(startSceneName != "")
        {
            SceneManager.LoadScene(startSceneName);
        }
        
    }

    public void OnExitButton()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // �����Ϳ����� ���� ����
#endif
    }
}

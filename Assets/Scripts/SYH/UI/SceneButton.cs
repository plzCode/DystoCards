using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    
    public void GoMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

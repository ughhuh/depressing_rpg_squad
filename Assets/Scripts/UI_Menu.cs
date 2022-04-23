using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Menu : MonoBehaviour
{
    public void OnStartClick()
    {
        SceneManager.LoadScene("Zone1");
    }

    public void OnQuitClick()
    {
        Application.Quit();
    }
    
    public void OnYesClick()
    {
        SceneManager.LoadScene("Zone1");
    }

    public void OnNoClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

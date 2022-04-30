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

    public void OnPauseClick()
    {
        Time.timeScale = 0f;
    }
    public void OnResumeClick()
    {
        Time.timeScale = 1f;
    }

    public void OnMainMenuClick()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}

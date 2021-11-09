using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(Constants.MainScene);
    }

    public void LoadSettings()
    {
        SceneManager.LoadScene(Constants.SettingsScene);
    }


    public void LoadLevel(int level)
    {
        SceneManager.LoadScene(Constants.Level + level);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

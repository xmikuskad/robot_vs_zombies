using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseBtn;

    [SerializeField]
    private GameObject pauseObj;

    private bool isPaused = false;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }
    public void PauseGame()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        pauseBtn.SetActive(!isPaused);
        pauseObj.SetActive(isPaused);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(Constants.MainScene);
    }
}

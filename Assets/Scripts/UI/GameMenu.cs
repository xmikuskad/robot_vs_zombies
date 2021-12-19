using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseBtn;
    [SerializeField]
    private GameObject pauseObj;
    [SerializeField]
    private GameObject winObj;
    [SerializeField]
    private GameObject loseObj;
    [SerializeField]
    private List<Animator> hearths;

    private bool isPaused = false;
    private bool isOver = false;

    private void Update()
    {
        if (isOver) return;
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

    public void WinGame()
    {
        Time.timeScale = 0;
        isOver = true;
        winObj.SetActive(true);
    }

    public void LoseGame()
    {
        Time.timeScale = 0;
        isOver = true;
        loseObj.SetActive(true);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(Constants.MainScene);
    }

    public void LoseHearth(int index)
    {
        if(index > hearths.Count)
        {
            Debug.LogError("There are not that many hearths rdy!");
            return;
        }
        hearths[index].SetTrigger(Constants.AnimLoseHearth);
    }
}

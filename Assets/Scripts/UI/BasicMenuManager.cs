using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

// Dotween animations copied from https://github.com/Noixelfer/MemoryGame/blob/master/Assets/Scripts/UIMainMenu.cs
public class BasicMenuManager : MonoBehaviour
{
    [SerializeField]
    private List<Button> buttons;
    [SerializeField]
    private CanvasGroup title;
    private List<Sequence> animationSequences = new List<Sequence>();

    private void Awake()
    {
        title.alpha = 0f;
        title.DOFade(1f, 1.8f).SetEase(Ease.InQuint);

        AnimateButtons();
    }

    private void AnimateButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].transform.localScale = Vector3.zero;
            AnimateButton(i, 1f +0.3f* i);
        }
    }

    private void AnimateButton(int index, float delay)
    {
        if (animationSequences.Count <= index)
        {
            animationSequences.Add(DOTween.Sequence());
        }
        else
        {
            if (animationSequences[index].IsPlaying())
            {
                animationSequences[index].Kill(true);
            }
        }

        var seq = animationSequences[index];
        var button = buttons[index];

        seq.Append(button.transform.DOScale(1, 0.1f));
        seq.Append(button.transform.DOPunchScale(Vector3.one * 0.6f, 0.3f, 6, 0.7f).SetEase(Ease.OutCirc));
        seq.PrependInterval(delay);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(Constants.MainScene);
    }

    public void LoadSettings()
    {
        SceneManager.LoadScene(Constants.SettingsScene);
    }

    public void LoadTutorial()
    {
        SceneManager.LoadScene(Constants.TutorialScene);
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

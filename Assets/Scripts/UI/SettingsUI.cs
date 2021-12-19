using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : BasicMenuManager
{
    [SerializeField]
    private float defaultGlobalVolume = 0.5f;

    [SerializeField]
    private Slider volumeSlider;
    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat(Constants.AudioVolume,defaultGlobalVolume);
        audioManager = GameObject.FindGameObjectWithTag(Constants.AudioManagerTag).GetComponent<AudioManager>();
    }

    public void SaveSettings()
    {
        var newValue = volumeSlider.value;
        audioManager.SaveGlobalVolume(newValue);
        LoadMainMenu();
    }
}

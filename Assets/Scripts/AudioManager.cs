using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField]
    private float defaultGlobalVolume = 0.5f;

    private float actualGlobalVolume;

    private static AudioManager _instance;

    public static AudioManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        actualGlobalVolume = PlayerPrefs.GetFloat(Constants.AudioVolume, defaultGlobalVolume);
    }

    public void SaveGlobalVolume(float volume)
    {
        PlayerPrefs.SetFloat(Constants.AudioVolume, volume);
        actualGlobalVolume = volume;
    }

    public void PlayClip(AudioClip clip, float volumeModifier)
    {
        AudioSource.PlayClipAtPoint(clip, this.transform.position,actualGlobalVolume*volumeModifier);
    }

}

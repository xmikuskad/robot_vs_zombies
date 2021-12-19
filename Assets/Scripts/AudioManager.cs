using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private float defaultGlobalVolume = 0.3f;

    private float actualGlobalVolume;

    private static AudioManager _instance;

    private AudioSource audioSource;

    [SerializeField]
    private float backgroundSoundModifier = 0.2f;

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
        audioSource = GetComponent<AudioSource>();

        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        actualGlobalVolume = PlayerPrefs.GetFloat(Constants.AudioVolume, defaultGlobalVolume);
        audioSource.volume = actualGlobalVolume * backgroundSoundModifier;
    }

    public void SaveGlobalVolume(float volume)
    {
        PlayerPrefs.SetFloat(Constants.AudioVolume, volume);
        actualGlobalVolume = volume;
        audioSource.volume = volume * backgroundSoundModifier;
    }

    public void PlayClip(AudioClip clip, float volumeModifier)
    {
        AudioSource.PlayClipAtPoint(clip, this.transform.position,actualGlobalVolume*volumeModifier);
    }

}

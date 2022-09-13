using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// @author: Neele Kemper
/// Class handles the audio settings for the music menu and the audio mixer.
/// </summary>
public class VolumeSettings : MonoBehaviour
{
    [SerializeField]
    private AudioMixer mixer;

    [SerializeField]
    private Slider themeSlider;
    [SerializeField]
    private Slider sfxSlider;
    [SerializeField]
    private Toggle muteButton;


    const string MIXER_MASTER = "MasterVolume";
    const string MIXER_THEME = "ThemeVolume";
    const string MIXER_SFX = "SFXVolume";

    private void Awake()
    {
        themeSlider.onValueChanged.AddListener(SetBackgroundVolume);
        sfxSlider.onValueChanged.AddListener(SetEffectVolume);
        muteButton.onValueChanged.AddListener(SetMasterVolume);
    }

    private void Start() {
        // load the player's saved settings.
        float masterVolume = PlayerPrefs.GetFloat(MIXER_MASTER, 0.0f);
        if(masterVolume == 0.0f)
        {
            muteButton.isOn = true;
        }
        else
        {
            muteButton.isOn = false;
        }
        themeSlider.value = PlayerPrefs.GetFloat(MIXER_THEME, themeSlider.value);
        sfxSlider.value = PlayerPrefs.GetFloat(MIXER_SFX, sfxSlider.value);
    }

    private void OnDisable() {
        // save the player's music settings.
        bool isMuted = !muteButton.isOn;
        if(isMuted)
        {
            PlayerPrefs.SetFloat(MIXER_MASTER, -80.0f);    
        }
        else
        {
            PlayerPrefs.SetFloat(MIXER_MASTER, 0.0f);    
        }
        PlayerPrefs.SetFloat(MIXER_THEME, themeSlider.value);
        PlayerPrefs.SetFloat(MIXER_SFX, sfxSlider.value);
        
    }

    /// <summary>
    /// @author: Neele Kemper
    /// Mute all audio sounds
    /// <param name="state">status of the toggle button</param>
    /// </summary>
    /// <returns></returns>
    private void SetMasterVolume(bool state)
    {   
        float value = 0.0f;
        if(!state)
        {
            // mute
            value = -80.0f;
        }
        mixer.SetFloat(MIXER_MASTER, value);
    }

    /// <summary>
    /// @author: Neele Kemper
    /// Set the volume of the theme music
    /// <param name="value">volume value</param>
    /// </summary>
    /// <returns></returns>
    private void SetBackgroundVolume(float value)
    {
        mixer.SetFloat(MIXER_THEME, Mathf.Log10(value) * 20);
    }

    /// <summary>
    /// @author: Neele Kemper
    /// Set the volume of the game effects
    /// <param name="value">volume value</param>
    /// </summary>
    /// <returns></returns>
    private void SetEffectVolume(float value)
    {   
        mixer.SetFloat(MIXER_SFX, Mathf.Log10(value) * 20);
    }
}

using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;

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
    private void SetMasterVolume(bool state)
    {   
        float value = 0.0f;
        if(!state)
        {
            value = -80.0f;
        }
        mixer.SetFloat(MIXER_MASTER, value);
    }


    private void SetBackgroundVolume(float value)
    {
        mixer.SetFloat(MIXER_THEME, Mathf.Log10(value) * 20);
    }


    private void SetEffectVolume(float value)
    {   
        mixer.SetFloat(MIXER_SFX, Mathf.Log10(value) * 20);
    }
}

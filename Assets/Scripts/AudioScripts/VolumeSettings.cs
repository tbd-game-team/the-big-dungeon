using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField]
    private AudioMixer mixer;

    [SerializeField]
    private Slider backgroundSlider;
    [SerializeField]
    private Slider effectSlider;
    [SerializeField]
    private Toggle muteButton;


    const string MIXER_MASTER = "MasterVolume";
    const string MIXER_BACKGROUND = "BackgroundVolume";
    const string MIXER_EFFECT = "EffectVolume";

    private void Awake()
    {
        backgroundSlider.onValueChanged.AddListener(SetBackgroundVolume);
        effectSlider.onValueChanged.AddListener(SetEffectVolume);
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
        backgroundSlider.value = PlayerPrefs.GetFloat(MIXER_BACKGROUND, backgroundSlider.value);
        effectSlider.value = PlayerPrefs.GetFloat(MIXER_EFFECT, effectSlider.value);
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
        PlayerPrefs.SetFloat(MIXER_BACKGROUND, backgroundSlider.value);
        PlayerPrefs.SetFloat(MIXER_EFFECT, effectSlider.value);
        
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
        mixer.SetFloat(MIXER_BACKGROUND, Mathf.Log10(value) * 20);
    }


    private void SetEffectVolume(float value)
    {   
        mixer.SetFloat(MIXER_EFFECT, Mathf.Log10(value) * 20);
    }
}

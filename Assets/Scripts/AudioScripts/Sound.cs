using UnityEngine.Audio;
using UnityEngine;



[System.Serializable]
public class Sound
{
    [SerializeField]
    public AudioClip clip;

    [SerializeField]
    public string name;

    [SerializeField]
    public AudioMixerGroup outputAudioMixerGroup;

    [SerializeField]
    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3.0f)]

    [SerializeField]
    public float pitch;

    [SerializeField]
    public bool loop;

    [SerializeField]
    public bool playOnAwake;
    [SerializeField]
    public bool bypassReverbZones;


    [HideInInspector]
    public AudioSource source;

}

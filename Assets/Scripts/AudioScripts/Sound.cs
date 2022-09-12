using UnityEngine.Audio;
using UnityEngine;

/// <summary>
/// @author: Neele Kemper
/// Class to set the audio source parameters.
/// </summary>
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
    
    [SerializeField]
    [Range(0.1f, 3.0f)]
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

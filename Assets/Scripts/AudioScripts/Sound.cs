using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound 
{
    public AudioClip clip;

    public string name;

    public AudioMixerGroup outputAudioMixerGroup;
    
    [Range(0f,1f)]
    public float volume;
    [Range(0.1f, 3.0f)]
    public float pitch;
    public bool loop;

    public bool bypassReverbZones;


    [HideInInspector]
    public AudioSource source;

}

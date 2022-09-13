using System;
using UnityEngine;

/// <summary>
/// @author: Neele Kemper
/// Class for more straightforward and clearer management of the audio sound. 
/// </summary>
public class AudioManager : MonoBehaviour
{
    [SerializeField]
    public Sound[] sounds;

    void Awake()
    {   
        // initialize the audio sounds
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.outputAudioMixerGroup;
        }
    }

    void Start()
    {   
        // Launch all sounds that should be launched at the start of the game.
        foreach(Sound s in sounds)
        {
            if(s.playOnAwake)
            {
                Play(s.name);
            }
        }
    }

    /// <summary>
    /// @author: Neele Kemper
    /// Start the sound.
    /// <param name="name">name of the sound</param>
    /// </summary>
    /// <returns></returns>
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }

    /// <summary>
    /// @author: Neele Kemper
    /// Stop the sound.
    /// <param name="name">name of the sound</param>
    /// </summary>
    /// <returns></returns>

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Stop();
    }

    /// <summary>
    /// @author: Neele Kemper
    /// Check if sound is being played
    /// <param name="name">name of the sound</param>
    /// </summary>
    /// <returns>true, if the sound is played, otherwise false</returns>

    public bool IsPlaying(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return false;
        }
        return s.source.isPlaying;
    }
}

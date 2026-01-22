using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] sounds;
    void Awake()
    {    
        Instance = this;
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.time = s.time;
            s.source.loop = s.loop;
        }
    }
    public void Play (string clip)
    {
        Sound s = Array.Find(sounds, sound => sound.name == clip);
        if (s == null)
            return;
        s.source.Play();
    }
    public void Mute (string clip)
    {
        Sound s = Array.Find(sounds, sound => sound.name == clip);
        if (s == null)
            return;
        s.source.mute = !s.source.mute;
    }
}

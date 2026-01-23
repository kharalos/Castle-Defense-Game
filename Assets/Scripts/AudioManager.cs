using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] sounds;

    private void Awake()
    {    
        Instance = this;
        foreach (var s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.time = s.time;
            s.source.loop = s.loop;
        }
    }
    public void Play (ClipType clip)
    {
        var s = Array.Find(sounds, sound => sound.clipType == clip);
        s?.source.Play();
    }
    public void Mute (ClipType clip)
    {
        var s = Array.Find(sounds, sound => sound.clipType == clip);
        if (s == null) return;
        s.source.mute = !s.source.mute;
    }
}

using System;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    private static AudioManager _Audio_Manager;
    public static AudioManager Instance { get { return _Audio_Manager; } }

    public Sound[] sounds;

    private void Awake()
    {
        //Singleton
        if(_Audio_Manager == null)
        {
            _Audio_Manager = this;
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (Sound sound in sounds)
        {
            //Add AudioSource to AudioManager from list of Sounds
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.soundClip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
    }

    private void Start()
    {
        //Play main things... e.g. "Theme"
    }

    public void Play(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if(s != null)
        {
            s.source.Play();
        }
        else
        {
            Debug.LogWarning("AudioManager: " + soundName + " not found!");
        }
    }
}

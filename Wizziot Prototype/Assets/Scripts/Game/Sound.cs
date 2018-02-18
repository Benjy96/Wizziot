using UnityEngine;

[System.Serializable]
public class Sound {

    //Name and clip
    public string name;
    public AudioClip soundClip;

    //Configuration of sound
    [Range(0f, 1f)] public float volume;
    [Range(0.1f, 3f)] public float pitch;
    public bool loop;

    //GameObject component that houses the above sound "data"
    [HideInInspector] public AudioSource source;
}

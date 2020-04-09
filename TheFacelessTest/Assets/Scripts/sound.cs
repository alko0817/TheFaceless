using UnityEngine.Audio;
using UnityEngine;


[System.Serializable]
public class sound 
{
    [Tooltip("Make sure you typed-in the same name as the clip attached below")]
    public string name;

    public AudioClip clip;

    
    [Range(0f, 1f)]
    public float volume;

    [Tooltip("The pitch of every clip needs to be at least 1. If it is less, it will work only on special occasions")]
    [Range(.1f, 3f)]
    public float pitch;

    [Tooltip("Loop should be enabled only for ambient sounds and soundtracks")]
    public bool loop;

    public bool mute;

    [HideInInspector]
    public AudioSource source;


}

using UnityEngine.Audio;
using System;

using UnityEngine;

public class audioManager : MonoBehaviour
{
    //FindObjectOfType<audioManager>().Play("any sound"); copy-paste this line in any script, in any event you want to add sounds.


    public sound[] sounds;

    private void Awake()
    {
        foreach (sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

        }
    }

    private void Start()
    {
        Play("hallway");
    }

    public void Play (string name)
    {
        
        sound s = Array.Find(sounds, sound => sound.name == name);
        
        if (s == null)
        {
            Debug.LogWarning("sound: " + name + "not found");
            return;
        }

        s.source.Play();
    }
}

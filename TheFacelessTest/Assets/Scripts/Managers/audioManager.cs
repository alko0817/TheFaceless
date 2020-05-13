using UnityEngine.Audio;
using System;
using UnityEngine;

public class audioManager : MonoBehaviour
{
    //FindObjectOfType<audioManager>().Play("any sound" + the sound folder that it derives from); copy-paste this line in any script, in any event you want to add sounds.


    public sound[] Music;
    public sound[] Flashbacks;
    public sound[] Ambient;
    [Space]
    public string BackgroundMusic;
    [Space]
    public string[] BackgroundAmbient;
    [Space]
    [Tooltip("Disables all sounds")]
    public bool muteAll;
    [Tooltip("Enable if you want volume control over whole folders, instead of individual sounds")]
    public bool useMasterControls;
    [Space]
    [Range(0f, 1f)]
    public float MusicVolume;
    [Range(0f, 1f)]
    public float FlashBacksVolume;
    [Range(0f, 1f)]
    public float AmbientVolume;


    public static audioManager instance;

    private void Awake()
    {

        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);


        SetSounds(Music, MusicVolume);
        SetSounds(Flashbacks, FlashBacksVolume);
        SetSounds(Ambient, AmbientVolume);
    }

    private void Start()
    {
        foreach (string sound in BackgroundAmbient)
        {
            Play(sound, Ambient);
        }
        

        Play(BackgroundMusic, Music);
    }

    private void Update()
    {
        if (!useMasterControls) return;
        MasterVolume(Music, MusicVolume);
        MasterVolume(Flashbacks, FlashBacksVolume);
        MasterVolume(Ambient, AmbientVolume);
    }

    private void MasterVolume(sound [] sounds, float indexVolume)
    {
        foreach (sound s in sounds)
        {
            s.source.volume = indexVolume;
        }
    }
    private void SetSounds (sound [] folder, float indexVolume)
    {
        foreach (sound s in folder)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            if (useMasterControls) s.source.volume = indexVolume;
            else s.source.volume = s.volume;
        }
    }

    public void Play (string name, sound[] sounds)
    {
        if(muteAll)
        {
            return;
        }
        sound s = Array.Find(sounds, sound => sound.name == name);
        
        if (s == null)
        {
            Debug.LogWarning("sound: " + name + "not found");
            return;
        }

        s.source.Play();
    }

    public void StopPlaying(string sound, sound[] sounds)
    {
        sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volume / 2f, s.volume / 2f));
        s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitch / 2f, s.pitch / 2f));

        s.source.Stop();
    }
}

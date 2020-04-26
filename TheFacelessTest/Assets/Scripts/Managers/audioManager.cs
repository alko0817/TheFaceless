using UnityEngine.Audio;
using System;
using UnityEngine;

public class audioManager : MonoBehaviour
{
    //FindObjectOfType<audioManager>().Play("any sound" + the sound folder that it derives from); copy-paste this line in any script, in any event you want to add sounds.


    public sound[] Music;
    public sound[] PlayerEffects;
    public sound[] EnemyEffects;
    public sound[] EnvironmentEffects;
    public sound[] Ambient;
    [Space]
    [Tooltip("The music/ambience playing in the background at all times")]
    public string Background;
    [Tooltip("If the background sound is ambient, click this")]
    public bool PlayAmbient;
    [Space]
    [Tooltip("Disables all sounds")]
    public bool muteAll;
    [Tooltip("Enable if you want volume control over whole folders, instead of individual sounds")]
    public bool useMasterControls;
    [Space]
    [Range(0f, 1f)]
    public float MusicVolume;
    [Range(0f, 1f)]
    public float PlayerEffectsVolume;
    [Range(0f, 1f)]
    public float EnemyEffectsVolume;
    [Range(0f, 1f)]
    public float EnvironmentEffectsVolume;
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
        SetSounds(PlayerEffects, PlayerEffectsVolume);
        SetSounds(EnemyEffects, EnemyEffectsVolume);
        SetSounds(EnvironmentEffects, EnvironmentEffectsVolume);
        SetSounds(Ambient, AmbientVolume);
    }

    private void Start()
    {
        if (PlayAmbient) Play(Background, Ambient);

        Play(Background, Music);
    }

    private void Update()
    {
        if (!useMasterControls) return;
        MasterVolume(Music, MusicVolume);
        MasterVolume(PlayerEffects, PlayerEffectsVolume);
        MasterVolume(EnemyEffects, EnemyEffectsVolume);
        MasterVolume(EnvironmentEffects, EnvironmentEffectsVolume);
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

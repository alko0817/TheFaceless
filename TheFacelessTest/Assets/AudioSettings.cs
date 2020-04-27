using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    internal audioManager manager;
    public Slider[] sliders;

    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<audioManager>();
        sliders[0].value = manager.MusicVolume;
        sliders[1].value = manager.PlayerEffectsVolume;
        sliders[2].value = manager.EnemyEffectsVolume;
        sliders[3].value = manager.EnvironmentEffectsVolume;
        sliders[4].value = manager.AmbientVolume;
    }
    
    public void SetMusic (float value)
    {
        manager.MusicVolume = value;
    }

    public void SetPlayerEffects (float value)
    {
        manager.PlayerEffectsVolume = value;
    }

    public void SetEnemyEffects (float value)
    {
        manager.EnemyEffectsVolume = value;
    }

    public void SetEnvironmentEffects (float value)
    {
        manager.EnvironmentEffectsVolume = value;
    }

    public void SetAmbient (float value)
    {
        manager.AmbientVolume = value;
    }

}

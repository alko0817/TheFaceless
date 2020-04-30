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
        sliders[2].value = manager.AmbientVolume;
    }
    
    public void SetMusic (float value)
    {
        manager.MusicVolume = value;
    }

    public void SetAmbient (float value)
    {
        manager.AmbientVolume = value;
    }

}

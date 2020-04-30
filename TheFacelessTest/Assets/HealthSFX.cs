using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSFX : MonoBehaviour
{
    playerController controller;
    PlayerHealth health;
    AudioSource sound;
    public float beatThreshold = 65f;
    public float t1 = 50f;
    public float t2 = 35f;
    public float t3 = 20f;

    bool c1, c2, c3, c4, reset = false;
    private void Start()
    {
        health = GetComponent<PlayerHealth>();
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
        sound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        #region fuckThis

        if (health.currentHealth > beatThreshold)
        {
            sound.Stop();
            reset = SetCheck();
        }


        else if (health.currentHealth < beatThreshold && health.currentHealth > t1) //65HP - 50HP
        {
            if (!c1)
            {
                Play(controller.SlowBeat);
                c1 = SetCheck();
            }
        }


        else if (health.currentHealth < t1 && health.currentHealth > t2) //50HP - 35HP
        {
            if (!c2)
            {
                Play(controller.MediumSlowBeat);
                c2 = SetCheck();
            }
        }


        else if (health.currentHealth < t2 && health.currentHealth > t3) //35HP - 20HP
        {
            if (!c3)
            {
                Play(controller.MediumFastBeat);
                c3 = SetCheck();
            }
        }


        else if (health.currentHealth < t3) //>20HP
        {
            if (!c4)
            {
                Play(controller.FastBeat);
                c4 = SetCheck();
            }
        }


        #endregion
    }

    void Play (AudioClip clip)
    {
        sound.Stop();
        sound.clip = clip;
        sound.Play();
    }

    bool SetCheck()
    {
        c1 = false;
        c2 = false;
        c3 = false;
        c4 = false;

        return true;
    }
}

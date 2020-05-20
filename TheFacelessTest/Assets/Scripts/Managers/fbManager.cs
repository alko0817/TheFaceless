using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fbManager : MonoBehaviour
{
    public triggerDetails[] triggers;
    internal GameObject[] PlayerTriggers;
    private audioManager sounds;

    int current, running = -1;
    bool playing = false;

    private void Awake()
    {
        foreach (triggerDetails trigger in triggers)
        {
            trigger.animator = trigger.popUp.GetComponent<Animator>();
        }

        PlayerTriggers = GameObject.FindGameObjectsWithTag("Triggers");
        sounds = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<audioManager>();
    }

    private void Update()
    {
        current = CheckTriggers();
        if (current < 0) return;

        if (playing && running != current)
        {
            StopTrigger(running);
            StartCoroutine(PlayTrigger(current));
        }
        else StartCoroutine(PlayTrigger(current));
        
        running = current;
    }

    private int CheckTriggers()
    {
        foreach (GameObject trigger in PlayerTriggers)
        {
            bool triggered = trigger.GetComponent<newTrigger>().triggered;
            if (triggered)
            {
                return trigger.GetComponent<newTrigger>().GetTriggerIndex();
            }
        }

        return -1;
    }

    IEnumerator PlayTrigger (int index)
    {
        playing = true;
        triggers[index].played = true;
        triggers[index].animator.SetTrigger("fadeIn");
        sounds.Play(triggers[index].sound, sounds.Flashbacks);
        yield return new WaitForSeconds(triggers[index].duration);

        playing = false;
        triggers[index].animator.SetTrigger("fadeOut");
    }

    void StopTrigger (int index)
    {
        StopCoroutine(PlayTrigger(index));
        triggers[index].animator.SetTrigger("fadeOut");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newTrigger : MonoBehaviour
{
    public int localIndex;
    fbManager manager;
    GameObject player;
    audioManager sounds;
    Animator UIAnim;
    float fadeOutDelay;
    string sound;
    bool triggered = false;
    internal bool wait = false;

    private void Start()
    {
        manager = GameObject.Find("Flashbacks Manager").GetComponent<fbManager>();
        fadeOutDelay = manager.triggers[localIndex].duration;
        UIAnim = manager.triggers[localIndex].animator;
        sound = manager.triggers[localIndex].sound;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered)
        {
            if (other.gameObject.tag == player.tag)
            {
                triggered = true;
                wait = true;
                UIAnim.SetTrigger("fadeIn");
                sounds.Play(sound, sounds.Flashbacks);
                StartCoroutine("FadeOut");

            }
        }
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(fadeOutDelay);
        wait = false;
        UIAnim.SetTrigger("fadeOut");
    }
}


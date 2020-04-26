using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newTrigger : MonoBehaviour
{
    public int localIndex;
    public fbManager manager;
    GameObject player;
    audioManager sounds;
    Animator UIAnim;
    float fadeOutDelay;
    string sound;
    bool triggered = false;

    private void Start()
    {
        fadeOutDelay = manager.triggers[localIndex].duration;
        UIAnim = manager.triggers[localIndex].animator;
        sound = manager.triggers[localIndex].sound;
        player = GameObject.FindGameObjectWithTag("Player");
        sounds = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<audioManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered)
        {
            if (other.gameObject.tag == player.tag)
            {
                triggered = true;
                UIAnim.SetTrigger("fadeIn");
                sounds.Play(sound, sounds.EnvironmentEffects);
                StartCoroutine("FadeOut");

            }
        }
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(fadeOutDelay);

        UIAnim.SetTrigger("fadeOut");
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newTrigger : MonoBehaviour
{
    public int localIndex;
    public fbManager manager;
    GameObject player;
    Animator UIAnim;
    float fadeOutDelay;
    string sound;
    bool triggered = false;

    private void Start()
    {
        fadeOutDelay = manager.triggers[localIndex].duration;
        UIAnim = manager.triggers[localIndex].animator;
        sound = manager.triggers[localIndex].sound;
        player = GameObject.Find("vBasicController_Idle (1)");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered)
        {
            if (other.gameObject.tag == player.tag)
            {
                triggered = true;
                UIAnim.SetTrigger("fadeIn");
                FindObjectOfType<audioManager>().Play(sound);
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


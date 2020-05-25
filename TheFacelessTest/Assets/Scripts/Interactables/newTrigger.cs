using System.Collections;
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
    internal bool triggered = false;
    bool done = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        manager = GameObject.Find("Flashbacks Manager").GetComponent<fbManager>();
        sounds = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<audioManager>();
        //fadeOutDelay = manager.triggers[localIndex].duration;
        //UIAnim = manager.triggers[localIndex].animator;
        //sound = manager.triggers[localIndex].sound;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && !done) 
        {
            if (other.gameObject.tag == player.tag)
            {
                //triggered = true;
                done = true;
                StartCoroutine(Trigger());
                //UIAnim.SetTrigger("fadeIn");
                //StartCoroutine("FadeOut");
                //sounds.Play(sound, sounds.Flashbacks);

            }
        }
    }

    IEnumerator Trigger()
    {
        triggered = true;
        yield return new WaitForEndOfFrame();
        triggered = false;
    }
    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(fadeOutDelay);
        UIAnim.SetTrigger("fadeOut");
    }

    public int GetTriggerIndex ()
    {
        return localIndex;
    }
}


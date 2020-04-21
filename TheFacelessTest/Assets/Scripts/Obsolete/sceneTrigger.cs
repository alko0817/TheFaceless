using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class sceneTrigger : MonoBehaviour
{
    public GameObject player;
    public GameObject popUp;
    Animator UIAnim;
    public float fadeOutDelay = 1f;
    string sound;


    private void Start()
    {
        UIAnim = popUp.GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == player.tag)
        {
            UIAnim.SetTrigger("fadeIn");
            StartCoroutine("FadeOut");

        }
    }

    IEnumerator FadeOut ()
    {
        yield return new WaitForSeconds(fadeOutDelay);

        UIAnim.SetTrigger("fadeOut");
    }

}

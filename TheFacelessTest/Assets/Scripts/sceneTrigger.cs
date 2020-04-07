using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class sceneTrigger : MonoBehaviour
{
    public GameObject player;
   // public GameObject panel;
   // public TextMeshProUGUI text;
    public Animator UIAnim;
    public float fadeOutDelay = 1f;


    //private void Awake()
    //{
    //    panel.SetActive(false);
    //    text.enabled = false;
    //}

    //SOME UI ELEMENTS
    //ADD EFFECTS?
    //MAYBE TEACH PLAYERS HOW TO FIGHT HERE

    //WHAT ELSE???

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == player.tag)
        {
            Debug.Log("This is your story... " + player.tag);

            //panel.SetActive(true);
            //text.enabled = true;
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

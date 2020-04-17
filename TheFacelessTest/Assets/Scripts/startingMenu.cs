using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startingMenu : MonoBehaviour
{
    public Animator animator;
    public levelLoader loader;

    
    public void PlayGame ()
    {
        StartCoroutine("Delay");
        

    }

    IEnumerator Delay ()
    {
        animator.SetTrigger("play");
        yield return new WaitForSeconds(2f);
        loader.LoadLevel(1);
    }

    public void QuitGame ()
    {
        Application.Quit();
    }


    private void Start()
    {
        FindObjectOfType<audioManager>().Play("intro");
    }
}

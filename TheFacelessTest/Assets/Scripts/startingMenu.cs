using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startingMenu : MonoBehaviour
{
    public Animator animator;

    
    public void PlayGame ()
    {
        StartCoroutine("Delay");
        

    }

    IEnumerator Delay ()
    {
        animator.SetTrigger("play");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(1);
    }

    public void QuitGame ()
    {
        Application.Quit();
    }
}

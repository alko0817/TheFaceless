using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startingMenu : MonoBehaviour
{
    public Animator animator;
    public levelLoader loader;
    audioManager sounds;

    private void Start()
    {
        sounds = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<audioManager>();

        sounds.Play("intro", sounds.Music);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
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
}

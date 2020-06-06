using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverTurn : MonoBehaviour
{
    Animator anim;
    GameObject player;
    public GameObject e;
    bool canOpen = false;
    internal bool triggered = false;
    public GameObject flaskbackTrigger;
    AudioSource source;

    private void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        source = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player.CompareTag(other.tag))
        {
            canOpen = true;
            e.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        canOpen = false;
        e.SetActive(false);
    }

    private void Update()
    {
        if (canOpen)
        {
            if (Input.GetButtonDown("Interact"))
            {
                if (!triggered)
                {
                    source.Play();
                    triggered = true;
                    anim.SetTrigger("open");
                    e.SetActive(false);
                    if  (flaskbackTrigger != null)
                        flaskbackTrigger.SetActive(false);
                }
            }
        }
    }


}

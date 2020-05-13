using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverTurn : MonoBehaviour
{
    Animator anim;
    GameObject player;
    bool canOpen = false;
    internal bool triggered = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player.CompareTag(other.tag))
        {
            canOpen = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        canOpen = false;
    }

    private void Update()
    {
        if (canOpen)
        {
            if (Input.GetButtonDown("Interact"))
            {
                if (!triggered)
                {
                    triggered = true;
                    anim.SetTrigger("open");
                }
            }
        }
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openDoor : MonoBehaviour
{
    Animator anim;
    public GameObject player;
    public GameObject text;
    bool canOpen = false;
    bool isOpen = false;



    private void Start()
    {
        anim = GetComponentInParent<Animator>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player.CompareTag(other.tag))
        {
            canOpen = true;
            text.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        canOpen = false;
        text.SetActive(false);
    }

    private void Update()
    {
        if (canOpen)
        {
            
            if (Input.GetButtonDown("Interact") && !isOpen)
            {
                isOpen = true;
                anim.SetTrigger("open");
                text.SetActive(false);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class posterPop : MonoBehaviour
{
    GameObject player;
    public GameObject canvas;
    Animator anim;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = canvas.GetComponent<Animator>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player.CompareTag(other.tag))
        {
            anim.SetTrigger("play");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        anim.SetTrigger("exit");
    }

   
}

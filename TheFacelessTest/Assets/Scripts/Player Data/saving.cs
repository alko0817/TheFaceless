using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class saving : MonoBehaviour
{
    public saveLoader saver;
    GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player.CompareTag(other.tag))
        {
            saver.SavePlayer();

            Debug.Log("saved");
        }
    }
}

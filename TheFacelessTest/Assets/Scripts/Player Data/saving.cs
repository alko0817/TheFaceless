using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class saving : MonoBehaviour
{
    saveLoader saver;
    GameObject player;

    private void Start()
    {
        saver = GameObject.FindGameObjectWithTag("Saver").GetComponent<saveLoader>();
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

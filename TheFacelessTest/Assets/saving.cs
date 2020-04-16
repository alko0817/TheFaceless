using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class saving : MonoBehaviour
{
    public saveLoader saver;
    public GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        if (player.CompareTag(other.tag))
        {
            saver.SavePlayer();

            Debug.Log("saved");
        }
    }
}

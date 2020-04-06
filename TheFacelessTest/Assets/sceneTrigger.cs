using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sceneTrigger : MonoBehaviour
{
    public GameObject player;

    //SOME UI ELEMENTS
    //ADD EFFECTS?
    //MAYBE TEACH PLAYERS HOW TO FIGHT HERE

    //WHAT ELSE???

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == player.tag)
        {
            Debug.Log("This is your story... " + player.tag);
        }
    }
}

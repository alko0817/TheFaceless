using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class autoDoor : MonoBehaviour
{
    public Animator DoorAnim;
    public GameObject player;
    //bool isOpen = false;

    

    private void OnTriggerEnter(Collider other)
    {
        if (player.CompareTag(other.tag))
        {
            Open();
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Close();
    }

    void Open ()
    {
        DoorAnim.SetTrigger("open");
    }

    void Close ()
    {
        DoorAnim.SetTrigger("close");
    }
}

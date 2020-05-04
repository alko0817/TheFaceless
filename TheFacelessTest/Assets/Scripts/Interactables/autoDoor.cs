using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class autoDoor : MonoBehaviour
{
    Animator DoorAnim;
    GameObject player, detector;
    //bool isOpen = false;

    private void Start()
    {
        DoorAnim = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        detector = GameObject.FindGameObjectWithTag("Detector");
        Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), detector.GetComponent<Collider>(), true);
    }

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

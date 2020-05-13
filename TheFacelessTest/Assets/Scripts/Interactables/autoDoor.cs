using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class autoDoor : MonoBehaviour
{
    Animator DoorAnim;
    GameObject player, detector;
    public LeverTurn lever;

    [Tooltip("Set to true if this door has a lever")]
    public bool useLever = false;
    bool isOpen = false;

    private void Start()
    {
        DoorAnim = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        detector = GameObject.FindGameObjectWithTag("Detector");
        if (detector != null)
        {
            Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), detector.GetComponent<Collider>(), true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (useLever)
        {
            if (!lever.triggered) return;
        }

        if (player.CompareTag(other.tag))
        {
            Open();
            isOpen = true;
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isOpen) return;
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

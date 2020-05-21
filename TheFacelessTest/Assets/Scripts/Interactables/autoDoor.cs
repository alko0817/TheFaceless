using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class autoDoor : MonoBehaviour
{
    Animator DoorAnim;
    GameObject player, detector;
    public Material green, red;

    public GameObject arch1, arch2;
    MeshRenderer mesh1, mesh2;
    public LeverTurn lever;

    [Tooltip("Set to true if this door has a lever")]
    public bool useLever = false;
    bool isOpen = false;
    bool canOpen = false;

    private void Start()
    {
        DoorAnim = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        mesh1 = arch1.GetComponent<MeshRenderer>();
        mesh2 = arch2.GetComponent<MeshRenderer>();

        detector = GameObject.FindGameObjectWithTag("Detector");
        if (detector != null)
        {
            Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), detector.GetComponent<Collider>(), true);
        }
        if (useLever)
        {
            mesh1.material = red;
            mesh2.material = red;
        }
        else
        {
            mesh1.material = green;
            mesh2.material = green;
        }
    }


    private void Update()
    {
        if (useLever)
        {
            if (lever.triggered && !canOpen)
            {
                mesh1.material = green;
                mesh2.material = green;
                canOpen = true;
            }
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

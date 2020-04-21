using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chargeSpot : MonoBehaviour
{
    public GameObject player;
    private bool stepped = false;
    private float originCharge;
    public float charging = .5f;


    private void Start()
    {
        originCharge = player.GetComponent<playerController>().chargeRate;
    }
    private void Update()
    {
        if (stepped)
        {
            player.GetComponent<playerController>().chargeRate = charging;
            player.GetComponent<playerController>().Charge();
        }

        else player.GetComponent<playerController>().chargeRate = originCharge;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (player.CompareTag(other.tag))
        {
            stepped = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        stepped = false;
    }
}

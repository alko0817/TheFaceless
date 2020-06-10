using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class chargeSpot : MonoBehaviour
{
    private GameObject player;
    private bool stepped = false;
    [Range(.1f, 1f)]
    public float chargeRate = .1f;
    public float chargeDelay = .5f;
    float timer = 0f;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        if (stepped)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = chargeDelay;
                player.GetComponent<playerController>().Charge(chargeRate);
            }
        }
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

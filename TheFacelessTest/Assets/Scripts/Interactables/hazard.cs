using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hazard : MonoBehaviour
{
    private GameObject player;
    private PlayerHealth health;
    public int damagePerTick;
    bool stepped = false;
    public float tick = 1f;
    float timer;

    //maybe slow the player

    private void Start()
    {
        timer = tick;
        player = GameObject.FindGameObjectWithTag("Player");
        health = GameObject.FindGameObjectWithTag("Player Health").GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (stepped)
        {
            if (timer < 0)
            {
                health.Damage(damagePerTick);
                timer = tick;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthPack : MonoBehaviour
{
    GameObject player;
    public int healAmount;
    PlayerHealth health;
    bool canHeal = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        health = GameObject.Find("stateOfHealth").GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (health.currentHealth == health.maxHealth)
        {
            canHeal = false;
        }
        else canHeal = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player.CompareTag(other.tag))
        {
            if (canHeal)
            {
                health.Heal(healAmount);
                Destroy(gameObject);
            }

        }
    }
}

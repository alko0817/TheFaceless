using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthPack : MonoBehaviour
{
    GameObject player;
    public int healAmount;
    PlayerHealth health;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        health = GameObject.Find("stateOfHealth").GetComponent<PlayerHealth>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player.CompareTag(other.tag))
        {
            health.Heal(healAmount);
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthPack : MonoBehaviour
{
    GameObject player;
    public int healAmount;
    PlayerHealth health;
    bool canHeal = false;
    AudioSource pickUp;
    MeshRenderer mesh;
    bool healed = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        health = GameObject.Find("stateOfHealth").GetComponent<PlayerHealth>();
        pickUp = GetComponent<AudioSource>();
        mesh = GetComponent<MeshRenderer>();
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
            if (canHeal && !healed)
            {
                healed = true;
                health.Heal(healAmount);
                StartCoroutine(disable());

            }

        }
    }
    IEnumerator disable()
    {
        pickUp.Play();
        mesh.enabled = false;
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);

    }
} 

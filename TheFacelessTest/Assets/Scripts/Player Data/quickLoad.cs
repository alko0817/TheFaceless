using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class quickLoad : MonoBehaviour
{
    saveLoader loader;
    PlayerHealth health;
    bool ressed = false;

    private void Start()
    {
        loader = GameObject.FindGameObjectWithTag("Saver").GetComponent<saveLoader>();
        health = GameObject.FindGameObjectWithTag("Player Health").GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (health.dead && !ressed)
        {
            ressed = true;
            StartCoroutine(Ressurect());
        }
    }

    IEnumerator Ressurect()
    {
        yield return new WaitForSeconds(3f);
        GetComponent<playerController>().anim.SetBool("dead", false);
        health.dead = false;
        ressed = false;
        health.deathScreen.SetActive(false);
        health.ResetHealth();
        loader.LoadPlayer();
        
    }
}

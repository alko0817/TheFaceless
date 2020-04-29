using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class quickLoad : MonoBehaviour
{
    saveLoader loader;
    PlayerHealth health;

    private void Start()
    {
        loader = GameObject.FindGameObjectWithTag("Saver").GetComponent<saveLoader>();
        health = GameObject.FindGameObjectWithTag("Player Health").GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (health.dead)
        {
            StartCoroutine(Ressurect());
        }
    }

    IEnumerator Ressurect()
    {
        yield return new WaitForSeconds(2f);

        GetComponent<playerController>().anim.SetBool("dead", false);
        health.deathScreen.SetActive(false);
        health.dead = false;
        health.ResetHealth();
        loader.LoadPlayer();
        
    }
}

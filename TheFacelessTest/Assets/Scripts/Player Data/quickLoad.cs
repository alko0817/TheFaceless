using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class quickLoad : MonoBehaviour
{
    public saveLoader loader;
    public PlayerHealth health;

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
        health.deathScreen.SetActive(false);
        health.ResetHealth();
        health.dead = false;
        loader.LoadPlayer();
        
    }
}

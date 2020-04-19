using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class quickLoad : MonoBehaviour
{
    public saveLoader loader;
    public PlayerHealth health;

    private void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            loader.LoadPlayer();
            health.ResetHealth();
        }
    }
}

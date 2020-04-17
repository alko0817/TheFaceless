using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class quickLoad : MonoBehaviour
{
    public saveLoader loader;

    private void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            loader.LoadPlayer();
        }
    }
}

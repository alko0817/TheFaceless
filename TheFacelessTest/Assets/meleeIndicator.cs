using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using UnityEngine;
using UnityEngine.UI;

public class meleeIndicator : MonoBehaviour
{
    MeleeEnemy controller;
    public Image ind;
    [Range(.008f, .05f)]
    public float rate;

    private void Start()
    {
        controller = GetComponent<MeleeEnemy>();
        ind.enabled = false;
        ind.fillAmount = 0f;
    }

    private void Update()
    {
        ind.enabled = controller.attackThrown;

        if (controller.attackThrown)
        {
            ind.fillAmount += rate;
        }

        else ind.fillAmount = 0f;
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pressF : MonoBehaviour
{
    public GameObject text;
    public GameObject textEffect;
    public playerController player;
    bool enable;

    private void Update()
    {
        enable = player.canDischarge;

        if (enable)
        {
            text.SetActive(true);
            textEffect.SetActive(true);
        }

        else
        {
            text.SetActive(false);
            textEffect.SetActive(false);
        }
    }
}

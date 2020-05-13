using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioSource source;
    public AudioClip step;
    playerController con;

    public bool useStepInstead;

    private void Start()
    {
        con = GetComponentInParent<playerController>();

    }

    private void Update()
    {
        if (useStepInstead) return;

        if (con.speed > 2f && !con.stamina.drainingDodge && !con.jumping)
        {
            source.enabled = true;
            if (con.sprinting)
            {
                source.pitch = 1.2f;
            }

            else
            {
                source.pitch = .71f;
            }
        }
        else
        {
            source.enabled = false;
        }
    }

    public void Step()
    {
        if (!useStepInstead) return;
        source.PlayOneShot(step);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioSource source;
    public AudioClip step;
    [Space]
    public float jogPitch = .71f;
    public float runPitch = 1.2f;
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
                source.pitch = runPitch;
            }

            else
            {
                source.pitch = jogPitch;
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

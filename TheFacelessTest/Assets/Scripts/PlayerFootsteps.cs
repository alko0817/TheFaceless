using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioSource sprint;
    playerController con;

    private void Start()
    {
        con = GetComponentInParent<playerController>();

    }

    private void Update()
    {
        if (con.speed > 2f && !con.stamina.drainingDodge && !con.jumping)
        {
            sprint.enabled = true;
            if (con.sprinting)
            {
                sprint.pitch = 1.2f;
            }

            else
            {
                sprint.pitch = .71f;
            }
        }
        else
        {
            sprint.enabled = false;
        }
    }
}

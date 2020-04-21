using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyControl : MonoBehaviour
{
    Rigidbody rb;
    Animator anim;
    float velX;
    float velZ;
    AIBehaviour ai;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        ai = GetComponent<AIBehaviour>();


    }
    private void Update()
    {
        velX = Mathf.Clamp(rb.velocity.x, -1f, 1f);
        velZ = Mathf.Clamp(rb.velocity.z, 1f, 1f);

        anim.SetFloat("velocityX", velX);
        anim.SetFloat("velocityZ", velZ);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public GameObject player;
    public Animator anim;

    public Transform detectPoint;
    public LayerMask enemyLayer;
    public float attackRadius = .5f;

    public int combos = 0;
    public float lastClick;
    public bool attacking = false;

  
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Slash();
            
        }


        if (Input.GetButtonDown("Fire2"))
        {
            Stab();
        }


    }


    void Slash ()
    {
        //Animation
        anim.SetTrigger("isSlash");

        //Enemy Detect
        Physics.OverlapSphere(detectPoint.position, attackRadius, enemyLayer);
        //Damage output
    }

    void Stab()
    {
        anim.SetTrigger("isStab");
    }


    private void OnDrawGizmosSelected()
    {
        if (detectPoint == null) return;
        Gizmos.DrawWireSphere(detectPoint.position, attackRadius);
    }
}

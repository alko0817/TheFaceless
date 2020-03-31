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

    int combos = 0;
    float lastClick = 0f;
    public float attackDelay = 1.5f;
    public float nextAttack = 2f;
    float nextCombo = 0f;
    public bool attacking = false;

  
    void Update()
    {
       // Debug.Log("clicked " + lastClick);
        //Debug.Log("combo 2 is up " + nextCombo);
        Debug.Log("combo counter: " + combos);
        lastClick -= Time.deltaTime;
        nextCombo -= Time.deltaTime;

        if (lastClick <= 0)
        {
            if (Input.GetButtonDown("Fire1") && (combos == 0))
            {
                lastClick = attackDelay;
                Slash();
                nextCombo = nextAttack;

            }
            
        }

        if (nextCombo > 0)
        {
            if (Input.GetButtonDown("Fire1") && (combos == 1))
            {
                Slash();
            }
        }



        
        

        if (Input.GetButtonDown("Fire2"))
        {
            Stab();
        }


    }


    //void Slash2()
    //{
    //    anim.SetTrigger("isSlash2");
    //}

    void Slash ()
    {
        //ANIMATION PLAY & CYCLE
        switch (combos)
        {
            case 0:
                anim.SetTrigger("isSlash");
                break;

            case 1:
                anim.SetTrigger("isSlash2");
                combos = 0;
                break;

            default:
                Debug.LogWarning("nåt är fel... ");
                break;
        }
        //if (combos == 0)
        //{
        //    anim.SetTrigger("isSlash");
        //    combos = 1;
        //}

        //if (combos == 1)
        //{
        //    anim.SetTrigger("isSlash2");
        //    combos = 0;
        //}



        //ENEMY DETECT
        Physics.OverlapSphere(detectPoint.position, attackRadius, enemyLayer);

        //DPS

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
